using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class EmployeeInfoModel
    {
        public string id { get; set; }
        public string id_card { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public int age { get; set; }
        public string sex { get; set; }
        public string create_by { get; set; }
        public DateTime create_date { get; set; }
        public string update_by { get; set; }
        public DateTime update_date { get; set; }
        public string del { get; set; }
        public string id_emp { get; set; }
        public string str_create_date => create_date.ToString("dd/MM/yyyy HH:mm:ss");
        public string str_Update_date => update_date.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
