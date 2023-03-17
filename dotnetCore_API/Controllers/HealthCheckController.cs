using dotnetCore_API.Center.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/")]
    [ApiController]
    [AllowAnonymous]
    public class HealthCheckController : ControllerBase
    {
        private readonly IDBCenter _dbCon;
        public HealthCheckController(IDBCenter dbCon)
        {
            _dbCon = dbCon;
        }
        [HttpGet]
        [Route("HealthCheck")]
        public IActionResult HealthCheck()
        {
            try
            {
                return Ok(new { status = 200, success = true , message = $"Connect {Assembly.GetExecutingAssembly().GetName().Name} Success!"});
            }
            catch (Exception ex)
            {
                return NotFound(new { status = 500, success = false, message = $"{ex.Message} | Inner: {ex.InnerException?.Message}" });
            }
        }
        [HttpGet]
        [Route("CheckConnectDB")]
        public IActionResult CheckConnectDB() 
        {
            try
            {
                using (var con = _dbCon.GetConnection())
                {
                    con.Close();
                };
                return Ok(new { status = 200, success = true, message = $"Connect to Database Success!" });
            }
            catch (Exception ex)
            {
                return NotFound(new { status = 500, success = false, message = $"{ex.Message} | Inner: {ex.InnerException?.Message}" });
            }
        }
    }
}
