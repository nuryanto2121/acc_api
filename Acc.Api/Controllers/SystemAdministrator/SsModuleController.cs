using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Acc.Api.Controllers.SystemAdministrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsModuleController : ControllerBase//, IAPIController<SsModule>
    {
        private IConfiguration config;
        private ICrudService<SsModule, int> sysMenuService;
        public SsModuleController(IConfiguration configuration)
        {
            config = configuration;
            sysMenuService = new SsModuleService(configuration);
        }
        [HttpDelete("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete(string option_url, int line_no, [Required]int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetById(string option_url, int line_no, [Required]int id)
        {
            var output = new Output();
            output.Data = id;
            return Ok(output);
        }

        [HttpPost("GetList")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetList([FromBody] ParamList ModelList)
        {
            throw new NotImplementedException();
        }

        [HttpGet("datalist")]
        [APIAuthorizeAttribute]
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
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] SsModule Model)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Insert(Model);
            }
            catch (Exception ex)
            {
                output = Tools.Error(ex);
            }
            return Ok(output);
        }

        [HttpPut]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] SsModule Model)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Update(Model);
            }
            catch (Exception ex)
            {
                output = Tools.Error(ex);
            }
            return Ok(output);
        }
    }
}