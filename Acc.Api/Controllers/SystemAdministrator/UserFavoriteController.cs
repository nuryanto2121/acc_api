using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Models.SystemAdministrator;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.SystemAdministrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFavoriteController : ControllerBase
    {
        private IConfiguration config;
        private SsMenuFavoriteService ssMenuFavService;

        public UserFavoriteController(IConfiguration configuration)
        {
            config = configuration;
            ssMenuFavService = new SsMenuFavoriteService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Insert(ParamMenuFav Model)
        {
            var _result = new Output();
            try
            {
                _result = ssMenuFavService.Insert(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }

      
        [HttpDelete("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Delete(int id)
        {
            var _result = new Output();
            try
            {
                _result = ssMenuFavService.Delete(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }
    }
}