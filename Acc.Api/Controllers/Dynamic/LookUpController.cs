using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Authorize;
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
    [Route("[controller]")]
    [ApiController]
    public class LookUpController : ControllerBase
    {
        private IDynamicService DynService;
        private readonly HttpContext Context;

        public LookUpController(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            DynService = new DynamicService(configuration);
            Context = contextAccessor.HttpContext;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost("GetData")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] JObject Model)
        {
            var output = new Output();
            try
            {
                output = DynService.GetDataLookUp(Model, SQL.Method.Aggregate.LookUp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
    }
}