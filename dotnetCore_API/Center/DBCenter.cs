using dotnetCore_API.Center.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API.Center
{
    public class DBCenter : IDBCenter
    {
        private readonly string _cnStr;
        public DBCenter(IConfiguration config)
        {
            _cnStr = config.GetConnectionString("DefaultConnection");
        }
        public SqlConnection GetConnection() 
        {
            var connection = new SqlConnection(this._cnStr);
            connection.Open();
            return connection;
        }
        public bool BulkInsert(DataTable dt,string tableName,ref string ErrMsg)
        {
            try
            {
                var conn = GetConnection();
                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy objbulk = new SqlBulkCopy(conn);

                //assign Destination table name  
                objbulk.DestinationTableName = (!string.IsNullOrEmpty(tableName)? tableName : "");

                foreach (DataColumn dcPrepped in dt.Columns)
                {
                    objbulk.ColumnMappings.Add(dcPrepped.ColumnName, dcPrepped.ColumnName);
                }

                //insert bulk Records into DataBase.  
                objbulk.WriteToServer(dt);
                objbulk.Close();
                conn.Dispose();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }
        public bool BulkUpdate(List<string> data, ref string ErrMsg)
        {
            int res = 0;
            bool result = false;
            try
            {
                //string ConnectionString = ConfigurationManager.AppSettings["MySQL_ConnectionStrings"];
                string combinedString = string.Join(";", data);
                StringBuilder sCommand = new StringBuilder(combinedString);

                using (var conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(sCommand.ToString(), conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        res = cmd.ExecuteNonQuery();
                        result = res > 0 ? true : false;
                        conn.Dispose();
                        conn.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return result;
            }
        }
    }
}
