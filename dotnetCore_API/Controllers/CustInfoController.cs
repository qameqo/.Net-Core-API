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
    public class CustInfoController : ControllerBase
    {
        private readonly ICustomerInfoServices _cusServices;
        public CustInfoController(ICustomerInfoServices cusServices)
        {
            _cusServices = cusServices;
        }
        [HttpPost]
        [Route("GetCustomerInfo")]
        public IActionResult GetCustomerInfo(CustomerInfoModel data)
        {
            var response = new ResponseModel();
            try
            {
                response.data = _cusServices.GetCustomerInfo(data.id_emp);
                response.success = true;
                response.status = 200;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message.ToString();
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("AddCustomerInfo")]
        public async Task<IActionResult> AddCustomerInfo(CustomerInfoModel data)
        {
            return Ok(await _cusServices.AddCustomerInfo(data));
        }

        [HttpPost]
        [Route("ChangeCustomerInfo")]
        public async Task<IActionResult> ChangeCustomerInfo(CustomerInfoModel data)
        {
            return Ok(await _cusServices.ChangeCustomerInfo(data));
        }

        [HttpPost]
        [Route("DeleteCustomerInfo")]
        public async Task<IActionResult> DeleteCustomerInfo(CustomerInfoModel data)
        {
            return Ok(await _cusServices.DeleteCustomerInfo(data));
        }

        [HttpPost]
        [Route("RemoveCustomerInfo")]
        public async Task<IActionResult> RemoveCustomerInfo(CustomerInfoModel data)
        {
            return Ok(await _cusServices.RemoveCustomerInfo(data));
        }
    }
}
