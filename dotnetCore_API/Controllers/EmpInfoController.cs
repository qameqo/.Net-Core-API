using dotnetCore_API.Models;
using dotnetCore_API.Services;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class EmpInfoController : ControllerBase
    {
        private readonly IEmployeeInfoServices _cusServices;
        public EmpInfoController(IEmployeeInfoServices cusServices)
        {
            _cusServices = cusServices;
        }
        [HttpPost]
        [Route("GetEmployeeInfo")]
        public IActionResult GetEmployeeInfo(EmployeeInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                response.data = _cusServices.GetEmployeeInfo(data);
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

        [HttpPost]
        [Route("AddEmployeeInfo")]
        public async Task<IActionResult> AddEmployeeInfo(EmployeeInfoModel data)
        {
            return Ok(await _cusServices.AddEmployeeInfo(data));
        }

        [HttpPost]
        [Route("ChangeEmployeeInfo")]
        public async Task<IActionResult> ChangeEmployeeInfo(EmployeeInfoModel data)
        {
            return Ok(await _cusServices.ChangeEmployeeInfo(data));
        }

        [HttpPost]
        [Route("DeleteEmployeeInfo")]
        public async Task<IActionResult> DeleteEmployeeInfo(EmployeeInfoModel data)
        {
            return Ok(await _cusServices.DeleteEmployeeInfo(data));
        }

        [HttpPost]
        [Route("RemoveEmployeeInfo")]
        public async Task<IActionResult> RemoveEmployeeInfo(EmployeeInfoModel data)
        {
            return Ok(await _cusServices.RemoveEmployeeInfo(data));
        }
    }
}
