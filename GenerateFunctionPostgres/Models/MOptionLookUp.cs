using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateFunctionPostgres.Models
{
    public partial class MOptionLookUp
    {
        public int SS_OptionLookUp_ID { get; set; }
        public string OptionLookUpCd { get; set; }
        public string ColumnDB { get; set; }
        public string ViewName { get; set; }
        public string SourceField { get; set; }
        public string SourceWhere { get; set; }
        public string UserInput { get; set; }
        public string UserEdit { get; set; }
        public DateTime TimeInput { get; set; }
        public DateTime TimeEdit { get; set; }
        public string DisplayLookup { get; set; }
        public bool isLookupList { get; set; }
        public bool isAsyn { get; set; }
    }

    public partial class MOptionLookUpPostgres
    {
        public int ss_option_lookup_id { get; set; }
        public string option_lookup_cd { get; set; }
        public string column_db { get; set; }
        public string view_name { get; set; }
        public string source_field { get; set; }
        public string source_where { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_iput { get; set; }
        public DateTime time_edit { get; set; }
        public string display_lookup { get; set; }
        public bool is_lookup_list { get; set; }
        public bool is_asyn { get; set; }
    }
}
