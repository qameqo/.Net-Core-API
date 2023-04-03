using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class EvidenceModel
    {
        public string gu_id { get; set; }
        public string evidence_path { get; set; }
        public string filename { get; set; }
        public string create_by { get; set; }
        public DateTime create_date { get; set; }
        public string update_by { get; set; }
        public DateTime update_date { get; set; }
        public string id_leave { get; set; }
    }
}
