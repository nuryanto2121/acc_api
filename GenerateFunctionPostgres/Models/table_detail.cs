using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateFunctionPostgres.Models
{
    public class table_detail
    {
        public int detail_id { get; set; }
        public int header_id { get; set; }
        public int position { get; set; }
        public int row_no { get; set; }
        public string column_name { get; set; }
        public string column_label { get; set; }
        public string column_type { get; set; }
        public string column_value { get; set; }
        public int corder { get; set; }
        public int max_length { get; set; }
        public string lookup_cd { get; set; }
        public string lookup_table { get; set; }
        public string lookup_db { get; set; }
        public string lookup_db_descs { get; set; }
        public string lookup_db_parameter { get; set; }
        public string lookup_initial_where { get; set; }
        public string table_name { get; set; }
        public bool is_required { get; set; }
        public bool is_visible { get; set; }
        public bool is_protected { get; set; }
        public bool is_key { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public string cmaster_url { get; set; }
        public bool running_no_status { get; set; }
        public string running_cd_column_spec { get; set; }
        public string running_cd_table_name { get; set; }

    }
}
