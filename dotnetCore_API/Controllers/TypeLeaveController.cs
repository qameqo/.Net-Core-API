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
    public class TypeLeaveController : ControllerBase
    {
        private readonly ITypeOfLeaveServices _typeServices;
        public TypeLeaveController(ITypeOfLeaveServices type)
        {
            _typeServices = type;
        }

        [HttpPost]
        [Route("GetTypeOfLeave")]
        public IActionResult GetTypeOfLeave()
        {
            var response = new ResponseModel();
            try
            {
                response.data = _typeServices.GetTypeOfLeave();
                response.success = true;
                response.status = 200;
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
    }
}
