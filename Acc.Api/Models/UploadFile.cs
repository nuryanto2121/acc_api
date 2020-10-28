using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class UploadFile
    {
        [Required]
        public string modulecd { get; set; }
        public IFormFile images { get; set; }
        //public ICollection<IFormFile> images { get; set; }
    }

    public class Attachement
    {
        [Required]
        public int ss_chat_h_id { get; set; }
        public DateTime chat_date { get; set; }

        public IFormFile images { get; set; }
        //public ICollection<IFormFile> images { get; set; }
    }

    public class PortInFile
    {
        [Required]
        public string ss_portfolio_id { get; set; }
        [Required]
        public string ss_subportfolio_id { get; set; }
        [Required]
        public string user_input { get; set; }
        [Required]
        public IFormFile file_portin { get; set; }
        //public ICollection<IFormFile> images { get; set; }
    }

    public class ParamPortIn
    {
        public int ss_portfolio_id { get; set; }
        
        public int ss_subportfolio_id { get; set; }
        
        public string user_input { get; set; }
        public JObject data_port { get; set; }
    }
}
