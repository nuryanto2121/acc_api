﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// 
        /// </summary>
        /// <param name="Model"> parameter yg dipake : {"portfolio_id":"","subportfolio_id":"","doc_type":"","doc_no":""}</param>
        /// <returns></returns>
        [HttpPost("Check")]
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
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost("SaveHeader")]
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
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet]
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