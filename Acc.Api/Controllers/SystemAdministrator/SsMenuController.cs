using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Acc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsMenuController : ControllerBase//, IAPIController<SsMenu>
    {
        private IConfiguration config;
        private ICrudService<SsMenu, int> sysMenuService;
        public SsMenuController(IConfiguration configuration)
        {
            config = configuration;
            sysMenuService = new SsMenuService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option_url"></param>
        /// <param name="line_no"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete(string option_url, int line_no, [Required]int id)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Delete(id);
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
        /// <param name="option_url"></param>
        /// <param name="line_no"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetById(string option_url, int line_no, [Required]int id)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.GetDataBy(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

    

        [HttpGet("datalist")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetDataList(string initialwhere = "")
        {
            var output = new Output();
            try
            {
             

                output = sysMenuService.GetDataList(initialwhere);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] SsMenu Model)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Insert(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return Ok(output);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] SsMenu Model)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return Ok(output);
        }

        [HttpPost("Shortcut")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetList([FromBody] ParamList ModelList)
        {
            throw new NotImplementedException();
        }
    }
}