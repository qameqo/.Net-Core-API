using dotnetCore_API.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Center.Interfaces
{
    public interface IDBCenter
    {
        public SqlConnection GetConnection();
        public bool BulkInsert(DataTable dt, string tableName,ref string ErrMsg);
        public bool BulkUpdate(List<string> data, ref string ErrMsg);
    }
}
