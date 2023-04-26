using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveServices _leaveServices;
        private readonly IWebHostEnvironment _environment;
        public LeaveController(ILeaveServices leaveServices, IWebHostEnvironment environment)
        {
            _leaveServices = leaveServices;
            _environment = environment;
        }

        [HttpPost]
        [Route("GetListLeave")]
        public IActionResult GetListLeave(GetLeaveModel data)
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
        public IActionResult ChangeLeaveInfo([FromForm] List<LeaveModel> model)
        {
            return Ok(_leaveServices.ChangeLeave(model));
        }
        [HttpPost]
        [Route("DeleteLeaveInfo")]
        public IActionResult DeleteLeaveInfo([FromForm] List<LeaveModel> data)
        {
            return Ok(_leaveServices.DeleteLeave(data));
        }
        [HttpPost]
        [Route("AddListLeaveInfo")]
        public async Task<IActionResult> SaveListLeaveInfo([FromForm] List<LeaveModel> model)
        {
            return Ok(await _leaveServices.AddListLeave(model));
        }
    }
}
