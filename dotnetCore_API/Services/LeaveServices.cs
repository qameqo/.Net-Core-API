using dotnetCore_API.Center.Interfaces;
using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services
{
    public class LeaveServices : ILeaveServices
    {
        private readonly IDBCenter _dbConn;
        private readonly IEmployeeInfoServices _cusServices;
        public LeaveServices(IDBCenter dbConn, IEmployeeInfoServices cusServices)
        {
            _dbConn = dbConn;
            _cusServices = cusServices;
        }

        public List<LeaveModel> GetListLeave(LeaveModel data) 
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string command = "select a.*,b.name_th,b.name_eng,b.gu_id as type_gu_id,b.description from T_Leave as a inner join M_TypeofLeave b on b.gu_id = a.id_type";
                    if (!string.IsNullOrEmpty(data.id_emp))
                    {
                        var dataEmp = _cusServices.GetEmployeeInfo(data.id_emp);
                        if (dataEmp != null && dataEmp.Count > 0)
                        {
                            command += $" WHERE a.id_emp = '{dataEmp[0].id}'";
                        }
                        else
                        {
                            throw new Exception($"Data Employee Not Found");
                        }
                    }
                    SqlDataAdapter cmd = new SqlDataAdapter(command, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    cmd.Dispose();
                    con.Close();
                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                    {
                        throw new Exception($"Data Leave Not Found");
                    }
                }
                var obj = JsonConvert.SerializeObject(DS.Tables[0]);
                //.Where(x => DateTime.ParseExact(x.startdate, "yyyy-MM-ddTHH:mm:ss", null).Day >= DateTime.Now.Day)
                return JsonConvert.DeserializeObject<List<LeaveModel>>(obj.ToString()).OrderByDescending(x => x.create_date).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResponseModel AddLeave(List<LeaveModel> data)
        {
            var response = new ResponseModel();
            try
            {
                if (data != null && data.Count > 0)
                {
                    var GetEmp = _cusServices.GetEmployeeInfo(data[0].id_emp);
                    if (GetEmp.Count == 0)
                    {
                        throw new Exception("Data Employee Not Found");
                    }
                    EmployeeInfoModel emp = GetEmp.FirstOrDefault();
                    DataTable dt = new DataTable();

                    DataColumn myColumn = new DataColumn();
                    myColumn.DataType = System.Type.GetType("System.Guid");
                    myColumn.ColumnName = "gu_id";
                    dt.Columns.Add(myColumn);
                    dt.Columns.Add(new DataColumn("startdate"));
                    dt.Columns.Add(new DataColumn("enddate"));
                    dt.Columns.Add(new DataColumn("starttime"));
                    dt.Columns.Add(new DataColumn("endtime"));
                    dt.Columns.Add(new DataColumn("create_date"));
                    dt.Columns.Add(new DataColumn("create_by"));
                    dt.Columns.Add(new DataColumn("update_date"));
                    dt.Columns.Add(new DataColumn("update_by"));
                    myColumn = new DataColumn();
                    myColumn.DataType = System.Type.GetType("System.Guid");
                    myColumn.ColumnName = "id_type";
                    dt.Columns.Add(myColumn);
                    myColumn = new DataColumn();
                    myColumn.DataType = System.Type.GetType("System.Guid");
                    myColumn.ColumnName = "id_emp";
                    dt.Columns.Add(myColumn);

                    for (int i = 0; i < data.Count; i++)
                    {

                        string now = DateTime.Now.Year.ToString() + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("dd");
                        DateTime Start = DateTime.ParseExact(data[i].startdate, "yyyy-MM-dd", null);
                        DateTime dtNow = DateTime.ParseExact(now, "yyyy-MM-dd", null);
                        if(Start.Date < dtNow.Date)
                        {
                            throw new Exception("กรุณาเลือกวันที่เริ่มต้นให้เท่ากับวันที่ปัจจุบัน");
                        }

                        string Msg = string.Empty;
                        data[i].id_emp = emp.id;
                        data[i].startdate = ConvertFullDateTime(data[i].startdate);
                        data[i].enddate = ConvertFullDateTime(data[i].enddate);
                        //Check Date and Time Duplicate
                        var resCheckDTdup = CheckDateTimeDup(data[i],"ADD",ref Msg);
                        if (resCheckDTdup && string.IsNullOrEmpty(Msg))
                        {
                            DataRow dr = dt.NewRow();

                            dr["gu_id"] = Guid.NewGuid();
                            dr["startdate"] = data[i].startdate;
                            dr["enddate"] = data[i].enddate;
                            dr["starttime"] = data[i].starttime;
                            dr["endtime"] = data[i].endtime;
                            dr["create_date"] = DateTime.Now;
                            dr["create_by"] = emp.fname;
                            dr["update_date"] = DateTime.Now;
                            dr["update_by"] = emp.fname;
                            dr["id_type"] = data[i].id_type;
                            dr["id_emp"] = data[i].id_emp;

                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            throw new Exception(Msg);
                        }
                    }

                    string ErrMsg = string.Empty;
                    if (dt.Rows.Count > 0)
                    {
                        bool Add = _dbConn.BulkInsert(dt, "dbo.T_Leave", ref ErrMsg);
                        if (Add && string.IsNullOrEmpty(ErrMsg))
                        {
                            response.status = 200;
                            response.success = true;
                            response.message = "Add Data Success!";
                        }
                        else
                        {
                            response.status = 200;
                            response.success = false;
                            response.message = "Add Data Fail!";
                        }
                    }
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Please enter leave information.";
                }
                
                return response;
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.success = false;
                response.message = ex.Message.ToString();
                return response;
            }
        }
        public ResponseModel ChangeLeave(List<LeaveModel> data)
        {
            var response = new ResponseModel();
            try
            {
                if (data != null && data.Count > 0)
                {
                    var GetEmp = _cusServices.GetEmployeeInfo(data[0].id_emp);
                    if (GetEmp.Count == 0)
                    {
                        throw new Exception("Data Employee Not Found");
                    }
                    EmployeeInfoModel emp = GetEmp.FirstOrDefault();

                    string sqlcommand = @" UPDATE T_Leave set startdate = '{0}' , enddate = '{1}', starttime = '{2}', endtime = '{3}', update_date = '{4}', update_by = '{5}', id_type = '{6}', id_emp = '{7}' where gu_id  = '{8}'";
                    List<string> listUpdLeave = new List<string>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        string now = DateTime.Now.Year.ToString() + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("dd");
                        DateTime Start = DateTime.ParseExact(data[i].startdate, "yyyy-MM-dd", null);
                        DateTime dtNow = DateTime.ParseExact(now, "yyyy-MM-dd", null);
                        if (Start.Date < dtNow.Date)
                        {
                            throw new Exception("กรุณาเลือกวันที่เริ่มต้นให้เท่ากับวันที่ปัจจุบัน");
                        }

                        string Msg = string.Empty;
                        data[i].id_emp = emp.id;
                        data[i].startdate = ConvertFullDateTime(data[i].startdate);
                        data[i].enddate = ConvertFullDateTime(data[i].enddate);
                        //Check Date and Time Duplicate
                        var resCheckDTdup = CheckDateTimeDup(data[i] ,"EDIT", ref Msg);
                        if (resCheckDTdup && string.IsNullOrEmpty(Msg))
                        {
                            string UpdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("en-US"));
                            listUpdLeave.Add(string.Format(sqlcommand, data[i].startdate, data[i].enddate, data[i].starttime, data[i].endtime, UpdDate, emp.fname, data[i].id_type, data[i].id_emp, data[i].gu_id));
                        }
                        else
                        {
                            throw new Exception(Msg);
                        }
                    }

                    string ErrMsg = string.Empty;

                    if (listUpdLeave != null && listUpdLeave.Count > 0)
                    {
                        bool Update = _dbConn.BulkUpdate(listUpdLeave, ref ErrMsg);
                        if (Update && string.IsNullOrEmpty(ErrMsg))
                        {
                            response.status = 200;
                            response.success = true;
                            response.message = "Update Data Success!";
                        }
                        else
                        {
                            response.status = 200;
                            response.success = false;
                            response.message = "Update Data Fail!";
                        }
                    }
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Please enter leave information.";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.success = false;
                response.message = ex.Message.ToString();
                return response;
            }
        }
        public ResponseModel DeleteLeave(List<LeaveModel> data)
        {
            var response = new ResponseModel();
            try
            {
                if (data != null && data.Count > 0)
                {
                    var GetEmp = _cusServices.GetEmployeeInfo(data[0].id_emp);
                    if (GetEmp.Count == 0)
                    {
                        throw new Exception("Data Employee Not Found");
                    }
                    EmployeeInfoModel emp = GetEmp.FirstOrDefault();

                    string sqlcommand = @"DELETE FROM T_Leave WHERE gu_id = '{0}'";
                    List<string> listDelLeave = new List<string>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        listDelLeave.Add(string.Format(sqlcommand, data[i].gu_id));
                    }

                    string ErrMsg = string.Empty;

                    if (listDelLeave != null && listDelLeave.Count > 0)
                    {
                        bool Update = _dbConn.BulkUpdate(listDelLeave, ref ErrMsg);
                        if (Update && string.IsNullOrEmpty(ErrMsg))
                        {
                            response.status = 200;
                            response.success = true;
                            response.message = "Delete Data Success!";
                        }
                        else
                        {
                            response.status = 200;
                            response.success = false;
                            response.message = "Delete Data Fail!";
                        }
                    }
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Please enter leave information.";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.success = false;
                response.message = ex.Message.ToString();
                return response;
            }
        }
        private bool CheckDateTimeDup(LeaveModel data,string mode, ref string Msg)
        {
            try
            {
                var DS = new DataSet();
                bool res = false;
                using (var con = _dbConn.GetConnection())
                {
                    string command = $" SELECT * FROM T_Leave WHERE (('{data.startdate}' BETWEEN startdate and enddate) OR ('{data.enddate}' BETWEEN startdate and enddate)) ";
                    command += $" and (('{data.starttime}' BETWEEN starttime and endtime) OR ('{data.endtime}' BETWEEN starttime and endtime)) and id_emp = '{data.id_emp}'";

                    if (!string.IsNullOrEmpty(data.gu_id))
                    {
                        command += $" and gu_id != '{data.gu_id}'";
                    }

                    SqlDataAdapter cmd = new SqlDataAdapter(command, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    cmd.Dispose();
                    con.Close();
                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                    {
                        res = true;
                    }
                    else
                    {
                        Msg = $"วันที่: {ConvertShrotDateTime(data.startdate)} ถึง {ConvertShrotDateTime(data.enddate)} เวลา: {data.starttime}-{data.endtime} มีการลงลาแล้ว";
                        if (mode.ToUpper() == "EDIT")
                        {
                            Msg += " กรุณาเลือกรายการที่ต้องการแก้ไขให้ถูกต้อง";
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConvertFullDateTime(string dt)
        {
            return Convert.ToDateTime(dt).ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }
        private string ConvertShrotDateTime(string dt)
        {
            return Convert.ToDateTime(dt).ToString("yyyy-MM-dd");
        }
    }
}
