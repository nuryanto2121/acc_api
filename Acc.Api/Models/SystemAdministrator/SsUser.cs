﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsUser : BaseEntity
    {       
        public int ss_user_id { get; set; }
        public string user_id { get; set; }
        public int ss_group_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string email { get; set; }
        public string user_level { get; set; }
        public DateTime expired_date { get; set; }
        public string is_inactive { get; set; }
        public string job_title { get; set; }
        public string hand_phone { get; set; }        
        
        public int failed_attempts { get; set; }
        public DateTime last_change_password { get; set; }
        public string defualt_language { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }

    }
   
}