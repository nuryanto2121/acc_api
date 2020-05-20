using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateFunctionPostgres.Models
{
    public class table_header
    {
        public int header_id { get; set; }
        public string url { get; set; }
        public string file_name { get; set; }
        public string title { get; set; }
        public int line_no { get; set; }
        public int level { get; set; }
        //public string relationparameter { get; set; }
        public string sp_i { get; set; }
        public string sp_u { get; set; }
        public string sp_s { get; set; }
        public string sp_d { get; set; }
        public string sp_process { get; set; }
        public string table_name { get; set; }
        public string form_type { get; set; }
        public string relation_param { get; set; }
        public int module_seq { get; set; }
        public int event_seq { get; set; }
        public int option_seq { get; set; }
        public int page_master_seq { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public string relation_type { get; set; }
    }
}
