using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class TypeOfLeaveModel
    {
        public string gu_id { get; set; }
        public int id { get; set; }
        public string name_eng { get; set; }
        public string name_th { get; set; }
        public string description { get; set; }
        public DateTime create_date { get; set; }
        public string create_by { get; set; }
        public DateTime update_date { get; set; }
        public string update_by { get; set; }
        public string str_create_date => create_date.ToString("dd/MM/yyyy HH:mm:ss");
        public string str_update_date => update_date.ToString("dd/MM/yyyy HH:mm:ss");


    }
}
