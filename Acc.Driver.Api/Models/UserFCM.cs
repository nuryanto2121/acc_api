using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Driver.Api.Models
{
    public class UserFCM
    {
        public int ss_chat_user_fcm_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public string user_id { get; set; }
        public string fcm_token { get; set; }
        public string token { get; set; }

    }
}
