using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class AuthLogin
    {
        [Required]
        public string UserLog { get; set; }
        [Required]
        public string PassLog { get; set; }
        public string Device { get; set; }
        public string Captcha { get; set; }
    }
}
