using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsUser : BaseEntity
    {
        public string ss_portfolio_id { get; set; }
        public string ss_subportfolio_id { get; set; }
        public int ss_user_id { get; set; }
        public string user_id { get; set; }
        public string ss_group_id { get; set; }
        public string dashboard_url { get; set; }
        public int group_id { get; set; }
        public string group_descs { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string email { get; set; }
        public string address { get; set; }
        public string user_level { get; set; }
        public DateTime expired_date { get; set; }
        public string is_inactive { get; set; }
        public string job_title { get; set; }
        public string hand_phone { get; set; }
        public DateTime date_added { get; set; }
        public DateTime last_change_password { get; set; }
        public string default_language { get; set; }
        public string notes { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public int lastupdatestamp { get; set; }
        public int portfolio_id { get; set; }
        public string portfolio_name { get; set; }
        public int subportfolio_id { get; set; }
        public string file_name { get; set; }
        public string path_file { get; set; }



    }

    public class VmSsUser
    {
        public string portfolio_id { get; set; }
        public string user_id { get; set; }
        public string dashboard_url { get; set; }
        public string group_id { get; set; }
        public List<SsMenuUser> DataDetail { get; set; }
        public string user_input { get; set; }

    }

}
