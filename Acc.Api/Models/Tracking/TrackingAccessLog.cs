using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class TrackingAccessLog
    {
        public int ss_tracking_access_log_id { get; set; }
        public string ip_address { get; set; }
        public string access_with { get; set; }
        public string captcha { get; set; }

    }
}
