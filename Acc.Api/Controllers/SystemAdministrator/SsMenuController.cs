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
using Acc.Api.Authorize;

namespace Acc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsMenuController : ControllerBase//, IAPIController<SsMenu>
    {
        private IConfiguration config;
        private ICrudService<SsMenu, int> sysMenuService;
        private SsMenuService menuService;
        public SsMenuController(IConfiguration configuration)
        {
            config = configuration;
            sysMenuService = new SsMenuService(configuration);
            menuService = new SsMenuService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete([Required]int id)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.Delete(id, 123);
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
        /// <returns></returns>
        [HttpGet("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetById(int id)
        {
            var output = new Output();
            try
            {
                output = sysMenuService.GetDataBy(id, 1);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
       

        [HttpGet("GetMenuJson")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetMenuJson(string portfolio_id,string group_id)
        {
            var output = new Output();
            try
            {
                output = menuService.GetMenuJson(portfolio_id, group_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
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
        [APIAuthorizeAttribute]
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
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetList([FromBody] ParamList ModelList)
        {
            throw new NotImplementedException();
        }
    }
}