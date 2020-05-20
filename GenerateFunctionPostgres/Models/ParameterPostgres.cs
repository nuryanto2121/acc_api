using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateFunctionPostgres.Models
{
    public class ParameterPostgres
    {
        public string routine_name { get; set; }
        public string column_name { get; set; }
        public string data_type { get; set; }
        public int position { get; set; }
        public int max_length { get; set; }
        public string is_nullable { get; set; }
        public int precision { get; set; }
        public int scale { get; set; }
        public string default_value { get; set; }
    }
}
