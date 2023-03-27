using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveServices _leaveServices;
        public LeaveController(ILeaveServices leaveServices)
        {
            _leaveServices = leaveServices;
        }

        [HttpPost]
        [Route("GetListLeave")]
        public IActionResult GetListLeave(LeaveModel data)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                response.data = _leaveServices.GetListLeave(data);
                response.status = 200;
                response.success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.success = false;
                response.message = ex.Message.ToString();
                return BadRequest(response);
            }
        }
        [HttpPost]
        [Route("AddLeaveInfo")]
        public IActionResult SaveLeaveInfo(List<LeaveModel> data)
        {
            return Ok( _leaveServices.AddLeave(data));
        }
        [HttpPost]
        [Route("ChangeLeaveInfo")]
        public IActionResult ChangeLeaveInfo(List<LeaveModel> data)
        {
            return Ok(_leaveServices.ChangeLeave(data));
        }
        [HttpPost]
        [Route("DeleteLeaveInfo")]
        public IActionResult DeleteLeaveInfo(List<LeaveModel> data)
        {
            return Ok(_leaveServices.DeleteLeave(data));
        }
    }
}
