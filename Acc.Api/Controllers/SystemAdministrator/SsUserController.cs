using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.SystemAdministrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsUserController : ControllerBase//, IAPIController<SsUser>
    {
        private ICrudService<SsUser, int> SsUserService;
        public SsUserController(IConfiguration configuration)
        {
            SsUserService = new SsUserService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastupdatestamp"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete([Required] int id, [Required] int lastupdatestamp)
        {
            var output = new Output();
            try
            {
                SsUserService.Delete(id, lastupdatestamp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastupdatestamp"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetById([Required] int id, [Required] int lastupdatestamp)
        {
            var output = new Output();
            try
            {
                output = SsUserService.GetDataBy(id, lastupdatestamp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        [HttpPost("GetList")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetList([FromBody] ParamList ModelList)
        {
            var output = new DTResultListDyn<dynamic>();
            try
            {
                output = SsUserService.GetList(ModelList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] SsUser Model)
        {
            var output = new Output();
            try
            {
                SsUserService.Insert(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] SsUser Model)
        {
            var output = new Output();
            try
            {
                SsUserService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
    }
}