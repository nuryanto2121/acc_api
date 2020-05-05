using Microsoft.AspNetCore.Http;
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
}
