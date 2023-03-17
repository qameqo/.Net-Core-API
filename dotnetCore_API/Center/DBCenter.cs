using dotnetCore_API.Center.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }
}
