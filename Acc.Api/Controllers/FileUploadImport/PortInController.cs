using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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

namespace Acc.Api.Controllers.FileUploadImport
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortInController : ControllerBase
    {
        private IHostingEnvironment _environment;
        private FileService FileService;
        private FunctionString fn;
        private PortInService PortService;
        public PortInController(IConfiguration configuration, IEmailService emailSender, IHostingEnvironment environment)
        {
            _environment = environment;
            fn = new FunctionString(Tools.ConnectionString(configuration));
            FileService = new FileService(configuration, environment);
            PortService = new PortInService(configuration, emailSender, environment);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portinFile"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<Output> UploadFile([FromForm]PortInFile portinFile)
        //public async Task<IActionResult> UploadFile([FromBody] PortInFile portinFile)
        {
            var _result = new Output();
            try
            {               
                _result = await PortService.ReadDataExcelToDBNew(portinFile);            
            }
            catch (Exception ex)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
                _result = Tools.Error(ex);
            }
            return _result;

        }
    }
}