using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateFunctionPostgres.Models
{
    public class MOptionDB
    {
        public int ss_option_db_id { get; set; }
        public string option_url { get; set; }
        public string method_api { get; set; }
        public string sp { get; set; }
        public int line_no { get; set; }
        public string table_name { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public string method_vue { get; set; }
        public int order_save { get; set; }
        public int order_update { get; set; }
    }
}
