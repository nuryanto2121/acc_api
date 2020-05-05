using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ParamFunction
    {
        public string routine_name { get; set; }
        public string parameter_name { get; set; }
        public string data_type { get; set; }
        public int oridinal_position { get; set; }
    }
    public class FieldSource
    {
        public string table_name { get; set; }
        public int position { get; set; }
        public string column_name { get; set; }
        public string data_type { get; set; }
        public int? max_length { get; set; }
        public string is_nullable { get; set; }
        public int? precision { get; set; }
        public int? scale { get; set; }
        public string default_value { get; set; }
    }
    public class DefineColumn
    {
        public int subportfolio_id { get; set; }
        public string column_field { get; set; }
    }
}
