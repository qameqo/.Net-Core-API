using dotnetCore_API.Center.Interfaces;
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
    public class EvidenceServices : IEvidenceServices
    {
        private readonly IDBCenter _dbConn;
        private readonly IEmployeeInfoServices _cusServices;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EvidenceServices(IDBCenter dbConn, IEmployeeInfoServices cusServices, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbConn = dbConn;
            _cusServices = cusServices;
            _env = env;
            _httpContextAccessor = httpContextAccessor;

        }
        public string SetUrlUploads(string fileName)
        {
            string imageUrl = "";
            if (!string.IsNullOrEmpty(fileName))
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var scheme = request.Scheme;
                var host = request.Host.Value;
                var pathBase = request.PathBase;
                string baseUrl = $"{scheme}://{host}{pathBase}";
                string imagePath = Path.Combine("uploads", fileName);
                imageUrl = $"{baseUrl}/{imagePath}";
            }
            return imageUrl;

        }
        public bool SaveEvidence(string url,string fname,string id_leave,string guid,string filename, ref string ErrMsg)
        {
            bool result = false;
            try
            {
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"INSERT INTO T_Evidence (gu_id,evidence_path,filename,create_by,create_date,update_by,update_date,id_leave)
                    VALUES (@gu_id,@evidence_path,@filename,@crt_by,@crt_dt,@upd_by,@upd_dt,@id_leave)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@gu_id", guid);
                    cmd.Parameters.AddWithValue("@evidence_path", (!string.IsNullOrEmpty(url)) ? url : "");
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@crt_by", fname.Trim());
                    cmd.Parameters.AddWithValue("@crt_dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@upd_by", fname.Trim());
                    cmd.Parameters.AddWithValue("@upd_dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id_leave", id_leave.Trim());
                    res = cmd.ExecuteNonQuery();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return result;
            }
        }
        public bool UpdateEvidence(string guid,string update_by ,ref string ErrMsg)
        {
            bool result = false;
            try
            {
                int res;
                using (var con = _dbConn.GetConnection())
                {
                    string UpdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("en-US"));
                    string query = $" UPDATE T_Evidence set update_by = '{update_by}' , update_date = '{UpdDate}' where gu_id  = '{guid}'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    res = cmd.ExecuteNonQuery();
                    result = (res == 1) ? true : false;
                    con.Dispose();
                    con.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return result;
            }
        }
        public List<EvidenceModel> GetEvidence(string id_leave)
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string query = $"SELECT * FROM T_Evidence WHERE id_leave = '{id_leave}'";
                    SqlDataAdapter cmd = new SqlDataAdapter(query, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    cmd.Dispose();
                    con.Close();
                    //if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                    //{
                    //    throw new Exception($"Data Not Found");
                    //}
                }
                var obj = JsonConvert.SerializeObject(DS.Tables[0]);
                return JsonConvert.DeserializeObject<List<EvidenceModel>>(obj.ToString()).OrderBy(x => x.create_date).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EvidenceModel> GetEvidenceByGuid(string gu_id)
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string query = $"SELECT * FROM T_Evidence WHERE gu_id = '{gu_id}'";
                    SqlDataAdapter cmd = new SqlDataAdapter(query, con);
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    cmd.Fill(DS);
                    cmd.Dispose();
                    con.Close();
                }
                var obj = JsonConvert.SerializeObject(DS.Tables[0]);
                return JsonConvert.DeserializeObject<List<EvidenceModel>>(obj.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseModel> ChangeEvidence(ChangeEvidenceModel model)
        {
            var res = new ResponseModel();
            try
            {
                if (model != null)
                {
                    if (model.File != null && model.File.Length > 0)
                    {
                        var folderPath = Path.Combine(_env.WebRootPath, "uploads");

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var getEvi = GetEvidenceByGuid(model.gu_id);
                        if (getEvi != null && getEvi.Count > 0)
                        {
                            var imagePath = Path.Combine(folderPath, getEvi[0].filename);
                            if (File.Exists(imagePath))
                            {
                                File.Delete(imagePath);
                            }
                        }
                        else
                        {
                            throw new Exception("Data Evidence is Null");
                        }

                        var filePath = Path.Combine(folderPath, getEvi[0].filename);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.File.CopyToAsync(stream);
                        }
                        
                        string ErrEviMsg = "";
                        bool resevidence = UpdateEvidence(model.gu_id, model.update_by, ref ErrEviMsg);
                        if (!resevidence && ErrEviMsg != "")
                        {
                            throw new Exception(ErrEviMsg);
                        }
                        else
                        {
                            res.status = 200;
                            res.success = true;
                            res.message = "Change Image Success!";
                        }
                    }
                    else
                    {
                        throw new Exception("Data File Image is Null");
                    }
                }
                else
                {
                    throw new Exception("Please Enter Data Evidence");
                }
            }
            catch (Exception ex)
            {
                res.status = 500;
                res.success = false;
                res.message = ex.Message;
            }

            return res;
        }
    }
}
