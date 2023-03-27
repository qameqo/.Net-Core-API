using dotnetCore_API.Center;
using dotnetCore_API.Center.Interfaces;
using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services
{
    public class EmployeeInfoServices : IEmployeeInfoServices
    {
        private readonly IDBCenter _dbConn;
        public EmployeeInfoServices(IDBCenter dbConn)
        {
            _dbConn = dbConn;
        }

        public List<EmployeeInfoModel> GetEmployeeInfo(string idEmp) 
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"SELECT * FROM Employee_Info WHERE del = ''";
                    if (!string.IsNullOrEmpty(idEmp))
                    {
                        query += $" AND id_emp = '{idEmp}'";
                    }
                    SqlDataAdapter cmd = new SqlDataAdapter(query, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    cmd.Dispose();
                    con.Close();
                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0) 
                    {
                        throw new Exception($"Data Employee Not Found");
                    }
                }
                var obj = JsonConvert.SerializeObject(DS.Tables[0]);
                return JsonConvert.DeserializeObject<List<EmployeeInfoModel>>(obj.ToString()).OrderByDescending(x => x.create_date).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseModel> AddEmployeeInfo(EmployeeInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var GetlatestEmp = GetEmployeeInfo("");
                var lastEmp = GetlatestEmp.OrderByDescending(x => x.create_date).FirstOrDefault();
                string emp =  lastEmp.id_emp.Substring(0,3);
                int number = int.Parse(lastEmp.id_emp.Substring(3));

                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"INSERT INTO Employee_Info (id,id_card,fname,lname,age,sex,create_by,create_date,update_by,update_date,del,id_emp)
                    VALUES (@id,@id_card,@fname,@lname,@age,@sex,@crt_by,@crt_dt,@upd_by,@upd_dt,@del,@id_emp)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@id_card", data.id_card.Trim());
                    cmd.Parameters.AddWithValue("@fname", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@lname", data.lname.Trim());
                    cmd.Parameters.AddWithValue("@age", data.age);
                    cmd.Parameters.AddWithValue("@sex", data.sex.Trim());
                    cmd.Parameters.AddWithValue("@crt_by", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@crt_dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@upd_by", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@upd_dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@del", string.Empty);
                    cmd.Parameters.AddWithValue("@id_emp", emp + (number + 1).ToString());
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Add Data EmployeeInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Add Data EmployeeInfo fail!";
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
        public async Task<ResponseModel> ChangeEmployeeInfo(EmployeeInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"UPDATE Employee_Info SET fname = @fname, lname = @lname, age = @age, sex = @sex, update_by = @upd_by, update_date = @upd_dt 
                    WHERE id_emp = @id_emp";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_emp", data.id_emp.Trim());
                    cmd.Parameters.AddWithValue("@fname", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@lname", data.lname.Trim());
                    cmd.Parameters.AddWithValue("@age", data.age);
                    cmd.Parameters.AddWithValue("@sex", data.sex.Trim());
                    cmd.Parameters.AddWithValue("@upd_by", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@upd_dt", DateTime.Now);
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Update Data EmployeeInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Update Data EmployeeInfo fail!";
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
        public async Task<ResponseModel> DeleteEmployeeInfo(EmployeeInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"UPDATE Employee_Info SET del = @del WHERE id_emp = @id_emp";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_emp", (!string.IsNullOrEmpty(data.id_emp))? data.id_emp.Trim():"");
                    cmd.Parameters.AddWithValue("@del", 'X');
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Delete Data EmployeeInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Delete Data EmployeeInfo fail!";
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
        public async Task<ResponseModel> RemoveEmployeeInfo(EmployeeInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"DELETE FROM Employee_Info WHERE id_emp = @id_emp";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_emp", (!string.IsNullOrEmpty(data.id_emp)) ? data.id_emp.Trim() : "");
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Remove Data EmployeeInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Remove Data EmployeeInfo fail!";
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
    }
}
