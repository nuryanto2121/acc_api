using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.Sender
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private ChatSenderService chatService;
        public ChatController(IConfiguration configuration)
        {
            chatService = new ChatSenderService(configuration);
        }

        /// <summary>
        /// check pesan, ketika klik logo pesan.
        /// </summary>
        /// <param name="Model"> parameter yg dipake : {"portfolio_id":"","subportfolio_id":"","doc_type":"","doc_no":"","current_page":1}</param>
        /// <returns></returns>
        [HttpPost("Check")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Check(ChatSender Model)
        {
            var output = new Output();
            try
            {
                output = chatService.GetAllChat(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// bila check pesan data'a null maka save header
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost("SaveHeader")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult SaveHeader(ChatSender Model)
        {
            var output = new Output();
            try
            {
                output = chatService.SaveHeader(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// untuk kirim pesan
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Send(ChatDetail Model)
        {
            var output = new Output();
            try
            {
                output = chatService.SendChat(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }


        /// <summary>
        /// untuk refresh pesan baru
        /// </summary>
        /// <param name="id">id header chat</param>
        /// <param name="user_id">user_id dari yg menerima/login</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetChat(int id, string user_id)
        {
            var output = new Output();
            try
            {
                output = chatService.GetChat(id, user_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

    }
}