using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ITokenServices _token;
        public TokenController(IConfiguration config, ITokenServices token)
        {
            _config = config;
            _token = token;
        }
        [HttpPost]
        [Route("BuildToken")]
        public IActionResult BuildToken(BuildTokenModel data)
        {
            try
            {
                return Ok(_token.BuildToken(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
