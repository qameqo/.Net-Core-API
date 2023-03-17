using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models.Common
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public bool Authen { get; set; }
    }
}
