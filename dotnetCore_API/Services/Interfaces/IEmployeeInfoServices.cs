using dotnetCore_API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface IEmployeeInfoServices
    {
        public List<EmployeeInfoModel> GetEmployeeInfo(string idCard);
        public Task<ResponseModel> AddEmployeeInfo(EmployeeInfoModel data);
        public Task<ResponseModel> ChangeEmployeeInfo(EmployeeInfoModel data);
        public Task<ResponseModel> DeleteEmployeeInfo(EmployeeInfoModel data);
        public Task<ResponseModel> RemoveEmployeeInfo(EmployeeInfoModel data);
    }
}
