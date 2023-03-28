using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceServices _eviServices;
        private readonly IWebHostEnvironment _environment;
        public EvidenceController(IEvidenceServices eviServices, IWebHostEnvironment environment)
        {
            _eviServices = eviServices;
            _environment = environment;
        }

        [HttpPost]
        [Route("AddEvidence")]
        //[Consumes("multipart/form-data")]
        //[Produces("application/json")]
        public async Task<IActionResult> AddEvidence([FromForm] EvidenceModel data)
        {
            return Ok(await _eviServices.AddEvidence(data));
        }

        [HttpPost]
        [Route("EEvidence")]
        [AllowAnonymous]
        //[Consumes("multipart/form-data")]
        //[Produces("application/json")]
        public IActionResult EEvidence([FromForm] List<EvidenceModel> model)
        {
            string res = "";
            for (int i = 0; i < model.Count; i++)
            {
                if(model[i].Files.Count > 0)
                {
                    for (int t = 0; t < model[i].Files.Count; t++)
                    {
                        res += "Ok";
                    }
                }
            }
            return Ok(res);
        }
    }
}
