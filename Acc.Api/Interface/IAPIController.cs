using Microsoft.AspNetCore.Mvc;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Acc.Api.Interface
{
    public interface IAPIController<T>
    {        
        IActionResult GetById([Required]string option_url, [Required]int line_no, [Required]int id, [Required]int lastupdatestamp);
        IActionResult GetList([FromBody]ParamList ModelList);
        IActionResult Insert([FromBody]T Model);
        IActionResult Update([FromBody]T Model);
        IActionResult Delete([Required]string option_url, [Required]int line_no, [Required]int id, [Required]int lastupdatestamp);
    }
}
