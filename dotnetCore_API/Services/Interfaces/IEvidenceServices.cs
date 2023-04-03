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
        public string SetUrlUploads(string fileName);
        public bool SaveEvidence(string url, string fname, string id_leave,string guid,string filename, ref string ErrMsg);
        public List<EvidenceModel> GetEvidence(string id_leave);
    }
}
