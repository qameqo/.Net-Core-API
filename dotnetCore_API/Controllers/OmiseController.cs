using dotnetCore_API.Models;
using dotnetCore_API.Services;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OmiseController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IOmiseServices _omise;

        public OmiseController(IConfiguration config, IOmiseServices omise)
        {
            _config = config;
            _omise = omise;
        }
        [Route("PayCreditCard")]
        [HttpPost]
        public async Task<IActionResult> PaymentCredit_Card(ReqCreateTokenOmise input)
        {
            try
            {
                string token_Card = await _omise.OmiseTokenCredit(input);
                string charge_Card = await _omise.OmiseChargeCredit(token_Card);
                return Ok(charge_Card);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [Route("PayTrueWallet")]
        [HttpPost]
        public async Task<IActionResult> PaymentTrueWallet()
        {
            try
            {
                string token_wallet = await _omise.OmiseGetSourceTrueWallet();
                string charge_wallet = await _omise.OmiseChargeTrueWallet(token_wallet);
                return Ok(charge_wallet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [Route("GenQR")]
        [HttpPost]
        public IActionResult GenQR(QRModelReq req)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                response.status = 200;
                response.success = true;
                response.data = _omise.GenerateQRCode(req);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.success = false;
                response.message = ex.Message;
                response.error = ex.StackTrace;
                return BadRequest(response);
            }
        }
    }
}
