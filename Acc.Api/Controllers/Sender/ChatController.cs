using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Hosting;
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
        private IHostingEnvironment _environment;
        public ChatController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
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
        /// untuk delete header dan detail2nya
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult DeleteHeader(int id)
        {
            var output = new Output();
            try
            {
                output = chatService.DeleteHeader(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

       

        /// <summary>
        /// ini utk tambah user di chat, `AddUser : adm,otn` dan seterusnya
        /// </summary>
        /// <param name="ID"> row id chat</param>
        /// <param name="AddUser">AddUser : adm,otn</param>
        /// <returns></returns>
        [HttpPut("AddRemoveUser/{ID}")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult AddUser(int ID, string AddUser)
        {
            var output = new Output();
            try
            {
                output = chatService.AddUser(ID, AddUser);
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
        public async Task<IActionResult> SendAsync(ChatDetail Model)
        {
            var output = new Output();
            try
            {
                output =await chatService.SendChatAsync(Model);
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
        public async Task<IActionResult> GetChat(int id, string user_id)
        {
            var output = new Output();
            try
            {
                output = await chatService.GetChatAsync(id, user_id);
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
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost("Attachment")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        //public void UploadFile([FromForm]UploadFile uploadFile)
        public async Task<Output> Attachment([FromForm]Attachement uploadFile)
        {
            var po = new Output();
            try
            {
                var folderName = Path.Combine("Chat", "Attachment");
                var pathToSave = Path.Combine(_environment.WebRootPath, folderName);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                Dictionary<string, string> data = new Dictionary<string, string>();
                //foreach (var file in uploadFile.images)
                //{
                var file = uploadFile.images;
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string fileType = Path.GetExtension(fileName).ToLower();
                var nameunix = DateTime.Now.Ticks.ToString() + fileType;
                //var fullPath = Path.Combine(pathToSave, linkAttachment.ToString());
                var fullPath = Path.Combine(pathToSave, nameunix);
                var dbPath = Path.Combine(folderName, nameunix);

                ChatAttachment Model = new ChatAttachment();
                Model.chat_date = uploadFile.chat_date;
                Model.ss_portfolio_id = Tools.PortfolioId;
                Model.ss_chat_h_id = uploadFile.ss_chat_h_id;
                Model.file_type = fileType;
                Model.file_name = file.FileName;
                Model.path_file = dbPath;
                var dtt = await chatService.SaveAttachment(Model);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                data.Add("path", dbPath);
                data.Add("name", file.FileName);


                //}
                po.Data = data;

            }
            catch (Exception ex)
            {
                po.Error = true;
                po.Message = ex.Message;
            }

            return po;

        }

    }
}