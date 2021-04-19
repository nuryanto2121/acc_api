using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Controllers.InvoiceFunding
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceFundingController : ControllerBase
    {
        private InvoiceFundingService InvService;
        public InvoiceFundingController(IConfiguration configuration)
        {
            InvService = new InvoiceFundingService(configuration);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> DoV1(ParamInvoiceFunding Model)
        {
            var _result = new Output();
            try
            {
                _result = await InvService.GetInvoice(Model);
                if (_result.Error)
                {
                    return StatusCode(501, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return this.Ok(_result);
        }
    }
}
