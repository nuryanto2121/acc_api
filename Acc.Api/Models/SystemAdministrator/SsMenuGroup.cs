using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsMenuGroup
    {
        public int ss_menu_group_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public int ss_menu_id { get; set; }
        public int ss_group_id { get; set; }
        public bool? add_status { get; set; }
        public bool? edit_status { get; set; }
        public bool? delete_status { get; set; }
        public bool? view_status { get; set; }
        public bool? post_status { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }

    public class SsMenuUser
    {
        public int ss_menu_group_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public string portfolio_id { get; set; }
        public int ss_menu_id { get; set; }
        public string user_id { get; set; }
        public bool? add_status { get; set; }
        public bool? edit_status { get; set; }
        public bool? delete_status { get; set; }
        public bool? view_status { get; set; }
        public bool? post_status { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }
}
