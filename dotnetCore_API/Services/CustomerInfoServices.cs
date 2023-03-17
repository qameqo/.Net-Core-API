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
    public class CustomerInfoServices : ICustomerInfoServices
    {
        private readonly IDBCenter _dbConn;
        public CustomerInfoServices(IDBCenter dbConn)
        {
            _dbConn = dbConn;
        }

        public List<CustomerInfoModel> GetCustomerInfo(string idCard) 
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"SELECT * FROM Customer_Info WHERE del = ''";
                    if (!string.IsNullOrEmpty(idCard))
                    {
                        query += $" AND id_card = '{idCard}'";
                    }
                    SqlDataAdapter cmd = new SqlDataAdapter(query, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    if (DS == null && DS.Tables.Count < 1)
                    {
                        throw new Exception($"Data Not Found");
                    }
                    con.Close();
                }
                var obj = JsonConvert.SerializeObject(DS.Tables[0]);
                return JsonConvert.DeserializeObject<List<CustomerInfoModel>>(obj.ToString()).OrderByDescending(x => x.create_date).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseModel> AddCustomerInfo(CustomerInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"INSERT INTO Customer_Info (id,id_card,fname,lname,age,sex,create_by,create_date,update_by,update_date,del)
                    VALUES (@id,@id_card,@fname,@lname,@age,@sex,@crt_by,@crt_dt,@upd_by,@upd_dt,@del)";
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
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Add Data CustomerInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Add Data CustomerInfo fail!";
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
        public async Task<ResponseModel> ChangeCustomerInfo(CustomerInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"UPDATE Customer_Info SET fname = @fname, lname = @lname, age = @age, sex = @sex, update_by = @upd_by, update_date = @upd_dt 
                    WHERE id_card = @id_card";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_card", data.id_card.Trim());
                    cmd.Parameters.AddWithValue("@fname", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@lname", data.lname.Trim());
                    cmd.Parameters.AddWithValue("@age", data.age);
                    cmd.Parameters.AddWithValue("@sex", data.sex.Trim());
                    cmd.Parameters.AddWithValue("@upd_by", data.fname.Trim());
                    cmd.Parameters.AddWithValue("@upd_dt", DateTime.Now);
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Update Data CustomerInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Update Data CustomerInfo fail!";
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
        public async Task<ResponseModel> DeleteCustomerInfo(CustomerInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"UPDATE Customer_Info SET del = @del WHERE id_card = @id_card";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_card", (!string.IsNullOrEmpty(data.id_card))? data.id_card.Trim():"");
                    cmd.Parameters.AddWithValue("@del", 'X');
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Delete Data CustomerInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Delete Data CustomerInfo fail!";
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
        public async Task<ResponseModel> RemoveCustomerInfo(CustomerInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                var result = false;
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"DELETE FROM Customer_Info WHERE id_card = @id_card";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_card", (!string.IsNullOrEmpty(data.id_card)) ? data.id_card.Trim() : "");
                    res = await cmd.ExecuteNonQueryAsync();
                    result = (res == 1) ? true : false;
                    con.Close();
                }
                if (result)
                {
                    response.status = 200;
                    response.success = true;
                    response.message = "Remove Data CustomerInfo Success!";
                }
                else
                {
                    response.status = 200;
                    response.success = false;
                    response.message = "Remove Data CustomerInfo fail!";
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
