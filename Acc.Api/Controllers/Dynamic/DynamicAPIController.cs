using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace Acc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicAPIController : ControllerBase, IAPIController<JObject>
    {
        private IDynamicService DynService;
        private readonly HttpContext Context;
        public DynamicAPIController(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            DynService = new DynamicService(configuration);
            Context = contextAccessor.HttpContext;
        }

        /// <summary>
        /// Dynamic API untuk Delete
        /// </summary>
        /// <param name="option_url">url utk dapat nama table/SP yg di exec</param>
        /// <param name="line_no"></param>
        /// <param name="id">ini id data yg ingin diGet</param>
        /// <param name="lastupdatestamp"></param>
        /// <returns></returns>
        [HttpDelete]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete([Required]string option_url, [Required]int line_no, [Required]int id, [Required]int lastupdatestamp)
        {
            var output = new Output();
            try
            {
                output = DynService.execute(id, lastupdatestamp, line_no, option_url, isDelete: true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        ///  Dynamic API untuk GetDataByID
        /// </summary>
        /// <param name="option_url">url utk dapat nama table/SP yg di exec</param>
        /// <param name="line_no"></param>
        /// <param name="id">ini id data yg ingin diGet</param>
        /// <param name="lastupdatestamp">ini xmin data yg ingin diGet</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetById([Required]string option_url, [Required]int line_no, [Required]int id, [Required]int lastupdatestamp)
        {
            var output = new Output();
            try
            {
                output = DynService.execute(id, lastupdatestamp, line_no, option_url);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// Dynamic API untuk GetList
        /// </summary>
        /// <param name="ModelList"></param>
        /// <returns></returns>
        [HttpPost("GetList")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetList([FromBody] ParamList ModelList)
        {
            //throw new NotImplementedException();
            var output = new DTResultListDyn<dynamic>();
            try
            {
                output = DynService.executeList(ModelList, SQL.Method.Aggregate.List);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// Dynamic API untuk GetListLookup
        /// </summary>
        /// <param name="ModelList"></param>
        /// <returns></returns>
        [HttpPost("GetListLookup")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetListLookup([FromBody] ParamLookupList ModelList)
        {
            //throw new NotImplementedException();
            var output = new DTResultListLookup<dynamic>();
            try
            {
                output = DynService.GetlistLookUp(ModelList, SQL.Method.Aggregate.List);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// Dynamic API untuk Insert
        /// </summary>
        /// <param name="Model">paramter wajib : option_url dan line_no ,selebih'a parameter utk function</param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] JObject Model)
        {
            var output = new Output();
            try
            {
                output = DynService.execute(Model, SQL.Method.Aggregate.Save);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// Dynamic API untuk Update
        /// </summary>
        /// <param name="Model">paramter wajib : option_url dan line_no ,selebih'a parameter utk function</param>
        /// <returns></returns>
        [HttpPut]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] JObject Model)
        {
            var output = new Output();
            try
            {
                output = DynService.execute(Model, SQL.Method.Aggregate.Update);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }



    }
}