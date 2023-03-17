using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class IndexModel
    {
        public string IP { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }
        public string Env { get; set; }
        public string Modified { get; set; }
    }
}
