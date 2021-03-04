using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Driver.Api.Models
{
    public partial class OptionDB
    {
        public int ss_option_db_id { get; set; }
        public string option_url { get; set; }
        public string method_api { get; set; }
        public string column_db { get; set; }
        public string sp { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public string source_field { get; set; }
        public string source_where { get; set; }
        public int line_no { get; set; }
        public string table_name { get; set; }
        public string field_param_output { get; set; }
    }

    public class OptionLookUp
    {
        public int ss_option_lookup_id { get; set; }
        public string option_lookup_cd { get; set; }
        public string column_db { get; set; }
        public string view_name { get; set; }
        public string source_field { get; set; }
        public string display_lookup { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }        
    }

    public class TablePortinFunction
    {
        public int ss_table_portin_function_id { get; set;}
        public string filename { get; set;}
        public string function_name { get; set;}
        public string user_input {get; set;}
        public string user_edit {get; set;}
        public DateTime time_input { get; set;}
        public DateTime time_edit { get; set;}
    }

    public class OptionFunction
    {
        public int ss_option_function_id { get; set; }
        public string option_function_cd { get; set; }
        public string module_cd { get; set; }
        public string sp_name { get; set; }
        public string sp_param { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }
}
