using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class UserSession
    {
        public int ss_user_session_id { get; set; }
        public string user_id { get; set; }
        public string token { get; set; }
        public DateTime? last_login { get; set; }
        public DateTime? expire_on { get; set; }
        public string ip_address { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }
}
