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
        public string SetUrlUploads(string fileName);
        public bool SaveEvidence(string url, string fname, string id_leave,string guid,string filename, ref string ErrMsg);
        public bool UpdateEvidence(string guid, string update_by, ref string ErrMsg);
        public List<EvidenceModel> GetEvidence(string id_leave);
        public Task<ResponseModel> ChangeEvidence(ChangeEvidenceModel model);
        public List<EvidenceModel> GetEvidenceByGuid(string gu_id);
        public bool DeleteEvidence(string guid, ref string ErrMsg);
        public ResponseModel RemoveEvidence(EvidenceModel model);
    }
}
