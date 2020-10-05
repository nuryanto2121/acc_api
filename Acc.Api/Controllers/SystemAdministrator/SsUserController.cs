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
using Microsoft.AspNetCore.Hosting;
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
        private SsUserService UserService;
        private SsUserService UserSer;
        private PortInService PortService;
        public SsUserController(IConfiguration configuration, IEmailService EmailSender, IHostingEnvironment environment)
        {
            SsUserService = new SsUserService(configuration);
            UserService = new SsUserService(configuration);
            PortService = new PortInService(configuration, EmailSender, environment);
            UserSer = new SsUserService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastupdatestamp"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [APIAuthorizeAttribute]
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
        [APIAuthorizeAttribute]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet("Json")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetMenuJson(string portfolio_id, string user_id)
        {
            var output = new Output();
            try
            {
                output = UserSer.GetMenuJson(portfolio_id, user_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        [HttpPost("GetList")]
        [APIAuthorizeAttribute]
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
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert([FromBody] SsUser Model)
        {
            var output = new Output();
            try
            {
                output = SsUserService.Insert(Model);
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
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Update([FromBody] SsUser Model)
        {
            var output = new Output();
            try
            {
                output = SsUserService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        [HttpPut("Menu")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult UpdateJson(VmSsUser Model)
        {
            var output = new Output();
            try
            {
                output = UserSer.Update(Model);
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
        /// <param name="portinFile"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult ChangePassword([FromBody]ChangePassword Model)
        //public async Task<IActionResult> UploadFile([FromBody] PortInFile portinFile)
        {
            var _result = new Output();
            try
            {
                //_result = await PortService.ReadDataExcelToDBUser(portinFile);
                _result = UserService.ChangePassword(Model);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return Ok(_result);

        }
    }
}