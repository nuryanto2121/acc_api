using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Enum;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.Dynamic
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicAPIMultiController : ControllerBase
    {
        private IDynamicService DynServeice;
        public DynamicAPIMultiController(IConfiguration configuration)
        {
            DynServeice = new DynamicService(configuration);
        }
        // [APIAuthorizeAttribute]
        [HttpPost("Delete")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete([FromBody] DynamicMultiParam Jmodel)
        {
            var _result = new Output();
            try
            {
                _result = DynServeice.executeMulti(Jmodel, SQL.Method.Aggregate.Delete);
                if (_result.Error)
                {
                    _result.Status = StatusCodes.Status500InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Helper.Tools.Error(ex));
            }
            return Ok(_result);

        }

        // [APIAuthorizeAttribute]
        [HttpPost("Insert")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] DynamicMultiParam Jmodel)
        {
            var _result = new Output();
            try
            {
                _result = DynServeice.executeMulti(Jmodel, SQL.Method.Aggregate.Save);
                if (_result.Error)
                {
                    _result.Status = StatusCodes.Status500InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Helper.Tools.Error(ex));
            }
            return Ok(_result);

        }

        // [APIAuthorizeAttribute]
        [HttpPut("Update")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] DynamicMultiParam Jmodel)
        {
            var _result = new Output();
            try
            {
                _result = DynServeice.executeMulti(Jmodel, SQL.Method.Aggregate.Update);
                if (_result.Error)
                {
                    _result.Status = StatusCodes.Status500InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Helper.Tools.Error(ex));
            }
            return Ok(_result);

        }

        // [APIAuthorizeAttribute]
        [HttpPost("Post")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Post([FromBody] DynamicMultiParam Jmodel)
        {
            var _result = new Output();
            try
            {
                _result = DynServeice.executeMultiPost(Jmodel, SQL.Method.Aggregate.Post);
                if (_result.Error)
                {
                    _result.Status = StatusCodes.Status500InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Helper.Tools.Error(ex));
            }
            return Ok(_result);

        }

        // [APIAuthorizeAttribute]
        [HttpPost("ProsesDataPostMulti")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult ProsesDataPostMulti([FromBody] DynamicMultiParam Jmodel)
        {
            var _result = new Output();
            try
            {
                _result = DynServeice.ProsesDataPostMulti(Jmodel, SQL.Method.Aggregate.Post);
                if (_result.Error)
                {
                    _result.Status = StatusCodes.Status500InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Helper.Tools.Error(ex));
            }
            return Ok(_result);
        }
    }
}