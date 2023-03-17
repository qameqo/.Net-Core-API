using dotnetCore_API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface ICustomerInfoServices
    {
        public List<CustomerInfoModel> GetCustomerInfo(string idCard);
        public Task<ResponseModel> AddCustomerInfo(CustomerInfoModel data);
        public Task<ResponseModel> ChangeCustomerInfo(CustomerInfoModel data);
        public Task<ResponseModel> DeleteCustomerInfo(CustomerInfoModel data);
        public Task<ResponseModel> RemoveCustomerInfo(CustomerInfoModel data);
    }
}
