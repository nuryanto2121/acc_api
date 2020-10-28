using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsMenu : BaseEntity
    {

        public int ss_menu_id { get; set; }

        [Required(ErrorMessage = "Please enter Title"), MaxLength(60)]
        public string title { get; set; }
        public string menu_url { get; set; }
        public string menu_type { get; set; }
        public int parent_menu_id { get; set; }
        public string icon_class { get; set; }    
        public int order_seq { get; set; }
        public int ss_module_id { get; set; }
        public int level_no { get; set; }
        public string menu_access { get; set; }
        public string user_input { get; set; }
        public string User_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public int lastupdatestamp { get; set; }
    }

    public class SsMenuTemp : BaseEntity
    {

        public int ss_menu_id { get; set; }

        public string title { get; set; }
        public string menu_url { get; set; }
        public string menu_type { get; set; }
        public int parent_menu_id { get; set; }
        public string icon_class { get; set; }
        public int order_seq { get; set; }
        public int ss_module_id { get; set; }
        public int level_no { get; set; }
        public string on_event { get; set; }
        public string user_input { get; set; }
        public DateTime time_input { get; set; }
        
    }
}
