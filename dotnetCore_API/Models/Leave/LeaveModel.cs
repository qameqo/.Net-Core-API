using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class LeaveModel
    {
        public string gu_id { get; set; }
        public string id { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string create_by { get; set; }
        public DateTime create_date { get; set; }
        public string update_by { get; set; }
        public DateTime update_date { get; set; }
        public string id_emp { get; set; }
        public string id_type { get; set; }
        public string str_create_date => create_date.ToString("dd/MM/yyyy HH:mm:ss");
        public string str_update_date => update_date.ToString("dd/MM/yyyy HH:mm:ss");
        public string str_start_date => Convert.ToDateTime(startdate).ToString("dd/MM/yyyy");
        public string str_end_date => Convert.ToDateTime(startdate).ToString("dd/MM/yyyy");
        public string name_th { get; set; }
        public string name_eng { get; set; }
        public string description { get; set; }
    }
}
