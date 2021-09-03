using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class UserManagement
    {
        public int ss_user_management_mobile_id
        {
            get; set;
        }
        public string user_name
        {
            get; set;
        }
        public string password
        {
            get; set;
        }
        public string email
        {
            get; set;
        }
        public DateTime? expired_date
        {
            get; set;
        }
        public string is_inactive
        {
            get; set;
        }
        public string job_title
        {
            get; set;
        }
        public string hand_phone
        {
            get; set;
        }
        public DateTime? last_change_password
        {
            get; set;
        }
        public string default_language
        {
            get; set;
        }
        public int lastupdatestamp
        {
            get; set;
        }
        public string user_edit
        {
            get; set;
        }
        public string user_input
        {
            get; set;
        }
        public string portfolio_id
        {
            get; set;
        }
        public string subportfolio_id
        {
            get; set;
        }
        public string file_name
        {
            get; set;
        }
        public string path_file
        {
            get; set;
        }
        public string address
        {
            get; set;
        }
        public string fcm_token_android
        {
            get; set;
        }
        public string fcm_token_ios
        {
            get; set;
        }


    }
}
