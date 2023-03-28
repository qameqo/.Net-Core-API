using dotnetCore_API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface IEvidenceServices
    {
        public Task<ResponseModel> AddEvidence(EvidenceModel data);
    }
}
