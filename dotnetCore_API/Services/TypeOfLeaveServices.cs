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
    public class TypeOfLeaveServices : ITypeOfLeaveServices
    {
        private readonly IDBCenter _dbConn;
        public TypeOfLeaveServices(IDBCenter dbConn)
        {
            _dbConn = dbConn;
        }
        public List<TypeOfLeaveModel> GetTypeOfLeave()
        {
            try
            {
                var DS = new DataSet();
                using (var con = _dbConn.GetConnection())
                {
                    string query = @"SELECT * FROM M_TypeofLeave";
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
                return JsonConvert.DeserializeObject<List<TypeOfLeaveModel>>(obj.ToString()).OrderBy(x => x.id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
