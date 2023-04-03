using dotnetCore_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface ILeaveServices
    {
        public List<LeaveModel> GetListLeave(LeaveModel data);
        public ResponseModel AddLeave(List<LeaveModel> data);
        public Task<ResponseModel> ChangeLeave(List<LeaveModel> data);
        public ResponseModel DeleteLeave(List<LeaveModel> data);
        public Task<ResponseModel> AddListLeave(List<LeaveModel> data);
    }
}
