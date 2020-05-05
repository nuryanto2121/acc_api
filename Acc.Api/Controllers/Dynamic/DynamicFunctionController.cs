using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Enum;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Acc.Api.Controllers.Dynamic
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicFunctionController : ControllerBase
    {
        private IDynamicService DynService;
        private readonly HttpContext Context;

        public DynamicFunctionController(IConfiguration configuration)
        {
            DynService = new DynamicService(configuration);
        }

        /// <summary>
        /// api untuk function yg didaftarkan di table function
        /// </summary>
        /// <param name="Jmodel">option_function_cd : required,module_cd : required, sisa'a parameter untuk functionnya</param>
        /// <returns></returns>
        [HttpPost("GetData")]
        public IActionResult GetData([FromBody] JObject Jmodel)
        {
            var output = new Output();
            try
            {
                output = DynService.executeFunction(Jmodel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);

        }
       
        //[HttpPost("GetDataList")]
        //public IActionResult GetDataList([FromBody] JObject Jmodel)
        //{
        //    var result = DynService.executeDataList(Jmodel, SQL.Method.Aggregate.DataList);
        //    return Ok(result);
        //}
    }
}