using dotnetCore_API.Center.Interfaces;
using dotnetCore_API.Common.Interfaces;
using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services
{
    public class LeaveServices : ILeaveServices
    {
        private readonly IDBCenter _dbConn;
        private readonly IEmployeeInfoServices _cusServices;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEvidenceServices _eviServices;
        private readonly IManageImage _mngImg;
        public LeaveServices(IDBCenter dbConn, IEmployeeInfoServices cusServices, IWebHostEnvironment env , IHttpContextAccessor httpContextAccessor,IEvidenceServices eviServices,IManageImage mngImg)
        {
            _dbConn = dbConn;
            _cusServices = cusServices;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _eviServices = eviServices;
            _mngImg = mngImg;
        }

        public List<GetLeaveModel> GetListLeave(GetLeaveModel data) 
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string command = "select a.*,b.name_th,b.name_eng,b.gu_id as type_gu_id,b.description from T_Leave as a inner join M_TypeofLeave b on b.gu_id = a.id_type";

                    if (!string.IsNullOrEmpty(data.startdate))
                    {
                        command += $" WHERE startdate >= '{data.startdate}'";
                    }
                    if (!string.IsNullOrEmpty(data.enddate))
                    {
                        command += $" AND enddate <= '{data.enddate}'";
                    }
                    if (!string.IsNullOrEmpty(data.id_emp))
                    {
                        EmployeeInfoModel model = new EmployeeInfoModel();
                        var dataEmp = _cusServices.GetEmployeeInfo(model);
                        if (dataEmp != null && dataEmp.Count > 0)
                        {
                            command += $" AND a.id_emp = '{dataEmp[0].id}'";
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
                return JsonConvert.DeserializeObject<List<GetLeaveModel>>(obj.ToString()).OrderByDescending(x => x.create_date).ToList();
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
                    EmployeeInfoModel model = new EmployeeInfoModel();
                    model.id_emp = data[0].id_emp;
                    var GetEmp = _cusServices.GetEmployeeInfo(model);
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
        public async Task<ResponseModel> AddListLeave(List<LeaveModel> data)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (data != null && data.Count > 0)
                {
                    EmployeeInfoModel model = new EmployeeInfoModel();
                    model.id_emp = data[0].id_emp;
                    var GetEmp = _cusServices.GetEmployeeInfo(model);
                    if (GetEmp.Count == 0)
                    {
                        throw new Exception("Data Employee Not Found");
                    }
                    EmployeeInfoModel emp = GetEmp.FirstOrDefault();
                    string insert = @"INSERT INTO T_Leave (gu_id,startdate,enddate,starttime,endtime,create_date,create_by,update_date,update_by,id_type,id_emp) VALUES
                    ('{0}', '{1}', '{2}', '{3}','{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')";


                    for (int i = 0; i < data.Count; i++)
                    {
                        string now = DateTime.Now.Year.ToString() + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("dd");
                        DateTime Start = DateTime.ParseExact(data[i].startdate, "yyyy-MM-dd", null);
                        DateTime dtNow = DateTime.ParseExact(now, "yyyy-MM-dd", null);
                        if (Start.Date < dtNow.Date)
                        {
                            throw new Exception("กรุณาเลือกวันที่เริ่มต้นให้เท่ากับวันที่ปัจจุบัน");
                        }
                        bool result = false;
                        int resleave = 0;
                        string Msg = string.Empty;
                        data[i].id_emp = emp.id;
                        data[i].startdate = ConvertFullDateTime(data[i].startdate);
                        data[i].enddate = ConvertFullDateTime(data[i].enddate);
                        string UpdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("en-US"));
                        //Check Date and Time Duplicate
                        var resCheckDTdup = CheckDateTimeDup(data[i], "ADD", ref Msg);
                        if (resCheckDTdup && string.IsNullOrEmpty(Msg))
                        {
                            //insert leave then save image and insert
                            string id_leave = Guid.NewGuid().ToString();
                            string query = string.Format(insert, id_leave, data[i].startdate, data[i].enddate, data[i].starttime, data[i].endtime, UpdDate, emp.fname, UpdDate, emp.fname, data[i].id_type, data[i].id_emp);
                            var con = _dbConn.GetConnection();
                            SqlCommand cmd = new SqlCommand(query, con);
                            resleave = await cmd.ExecuteNonQueryAsync();
                            result = (resleave == 1) ? true : false;
                            con.Dispose();
                            con.Close();
                            if (result)
                            {
                                //add image and insert data evidence
                                if (data[i].Files != null && data[i].Files.Count > 0)
                                {
                                    // Process the files
                                    foreach (var file in data[i].Files)
                                    {
                                        if (file != null && file.Length > 0)
                                        {
                                            string guid = Guid.NewGuid().ToString();
                                            var fileName = guid + Path.GetExtension(file.FileName);
                                            string ErrMsg = "";

                                            var folderPath = _mngImg.FolderPath("uploads");

                                            string filepath = Path.Combine(folderPath, fileName);
                                            var resUploadImage = _mngImg.UploadImage(filepath, file, ErrMsg);
                                            if (!resUploadImage.Result.Item1 && !string.IsNullOrEmpty(resUploadImage.Result.Item2))
                                            {
                                                throw new Exception(resUploadImage.Result.Item2);
                                            }

                                            string url = _eviServices.SetUrlUploads(fileName);
                                            bool resevidence = _eviServices.SaveEvidence(url, emp.fname,id_leave, guid, fileName, ref ErrMsg);
                                            if (!resevidence && ErrMsg != "")
                                            {
                                                throw new Exception(ErrMsg);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                response.status = 200;
                                response.success = false;
                                response.message = "Add Data LeaveInfo fail!";
                            }
                        }
                        else
                        {
                            throw new Exception(Msg);
                        }
                    }
                    response.status = 200;
                    response.success = true;
                    response.message = "Add Data LeaveInfo Success!";
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
                    EmployeeInfoModel model = new EmployeeInfoModel();
                    model.id_emp = data[0].id_emp;
                    var GetEmp = _cusServices.GetEmployeeInfo(model);
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
                        
                        var dataEvi = _eviServices.GetEvidence(data[i].gu_id);
                        if (dataEvi != null && dataEvi.Count > 0)
                        {
                            string sqlDelEvidence = @"DELETE FROM T_Evidence WHERE filename = '{0}'";
                            var folderPath = Path.Combine(_env.WebRootPath, "uploads");
                            foreach (var item in dataEvi)
                            {
                                string MsgErr = "";
                                var resDeleteImage = _mngImg.DeleteImage(item.filename, folderPath, ref MsgErr);
                                if (!resDeleteImage && !string.IsNullOrEmpty(MsgErr))
                                {
                                    throw new Exception(MsgErr);
                                }
                                else
                                {
                                    using (var _con = _dbConn.GetConnection())
                                    {
                                        SqlCommand cmd = new SqlCommand(string.Format(sqlDelEvidence, item.filename), _con);
                                        cmd.CommandType = CommandType.Text;
                                        cmd.ExecuteNonQuery();
                                    };
                                }
                            }
                        }
                        if (data[i].Files != null && data[i].Files.Count > 0)
                        {
                            foreach (var file in data[i].Files)
                            {
                                if (file != null && file.Length > 0)
                                {
                                    string guid = Guid.NewGuid().ToString();
                                    var fileName = guid + Path.GetExtension(file.FileName);
                                    string ErrSaveEviMsg = "";

                                    var folderPath = _mngImg.FolderPath("uploads");

                                    string filepath = Path.Combine(folderPath, fileName);
                                    var resUploadImage = _mngImg.UploadImage(filepath, file, ErrSaveEviMsg);
                                    if (!resUploadImage.Result.Item1 && !string.IsNullOrEmpty(resUploadImage.Result.Item2))
                                    {
                                        throw new Exception(resUploadImage.Result.Item2);
                                    }

                                    string url = _eviServices.SetUrlUploads(fileName);
                                    bool resevidence = _eviServices.SaveEvidence(url, emp.fname, data[i].gu_id, guid, fileName, ref ErrSaveEviMsg);
                                    if (!resevidence && ErrSaveEviMsg != "")
                                    {
                                        throw new Exception(ErrSaveEviMsg);
                                    }
                                }
                            }
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
                    EmployeeInfoModel model = new EmployeeInfoModel();
                    model.id_emp = data[0].id_emp;
                    var GetEmp = _cusServices.GetEmployeeInfo(model);
                    if (GetEmp.Count == 0)
                    {
                        throw new Exception("Data Employee Not Found");
                    }
                    EmployeeInfoModel emp = GetEmp.FirstOrDefault();

                    string sqlDelEvidence = @"DELETE FROM T_Evidence WHERE id_leave = '{0}'";
                    var GetEvidence = _eviServices.GetEvidence(data[0].gu_id);
                    if (GetEvidence != null && GetEvidence.Count > 0)
                    {
                        var _con = _dbConn.GetConnection();
                        SqlCommand cmd = new SqlCommand(string.Format(sqlDelEvidence, data[0].gu_id), _con);
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }

                    string sqlDelLeave = @"DELETE FROM T_Leave WHERE gu_id = '{0}'";
                    List<string> listDelLeave = new List<string>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        
                        listDelLeave.Add(string.Format(sqlDelLeave, data[i].gu_id));
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
                        string start = DateTime.ParseExact(data.startdate, "yyyy-MM-ddTHH:mm:ss.fff", null).ToString("yyyy-MM-dd");
                        string end = DateTime.ParseExact(data.enddate, "yyyy-MM-ddTHH:mm:ss.fff", null).ToString("yyyy-MM-dd");
                        Msg = $"วันที่: {start} ถึง {end} เวลา: {data.starttime}-{data.endtime} มีการลงลาแล้ว";
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
