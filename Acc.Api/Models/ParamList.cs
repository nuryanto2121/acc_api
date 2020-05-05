using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ParamList
    {
        [Required]
        public string option_url { get; set; }
        [Required]
        public int line_no { get; set; }
        public string user_id { get; set; }
        public string portfolio_id { get; set; }
        public string subportfolio_id { get; set; }
        public int current_page { get; set; }
        public int per_page { get; set; }
        public string param_where { get; set; }
        public string initial_where { get; set; }
        public string sort_field { get; set; }
        public string param_view { get; set; }
    }
    public class ParamLookupList
    {
        //[Required]
        //public string option_url { get; set; }
        //[Required]
        //public int line_no { get; set; }
        [Required]
        public string look_up_cd { get; set; }
        [Required]
        public string column_db { get; set; }
        public int current_page { get; set; }
        public int per_page { get; set; }
        public string param_where { get; set; }
        public string initial_where { get; set; }
        public string sort_field { get; set; }
        public string param_view { get; set; }
    }
}
