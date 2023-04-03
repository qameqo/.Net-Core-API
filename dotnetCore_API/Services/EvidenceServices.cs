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
        public async Task<ResponseModel> AddEvidence(EvidenceModel data)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (data.Files == null || data.Files.Count == 0)
                    throw new Exception("No file was selected.");

                // Process the files
                foreach (var file in data.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        var folderPath = Path.Combine(_env.WebRootPath, "uploads");

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var filePath = Path.Combine(folderPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        string url = SetUrlUploads(fileName);
                        string ErrMsg = "";
                        bool res = SaveEvidence(url,data.create_by,data.id_leave, Guid.NewGuid().ToString(), fileName, ref ErrMsg);
                        if (!res && ErrMsg != "")
                        {
                            throw new Exception(ErrMsg);
                        }
                    }
                }
                response.success = true;
                response.status = 200;
                response.message = "Add Image Success!";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
                response.status = 500;
            }
            return response;
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
    }
}
