using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class BuildTokenModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Issuer { get; set; }
        public string Roles { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
