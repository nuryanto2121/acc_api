using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    interface IDynamicAPI
    {
        IActionResult GetById([FromBody]JObject Jmodel);

        IActionResult GetList([FromBody]JObject Jmodel);

        IActionResult Insert([FromBody]JObject Jmodel);
        IActionResult Update([FromBody]JObject Jmodel);

        IActionResult Delete([FromBody]JObject Jmodel);
    }
}
