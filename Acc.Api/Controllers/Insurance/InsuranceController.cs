using Acc.Api.Authorize;
using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Acc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private IHostingEnvironment _environment;
        private FunctionString fn;
        private InsuranceService serviceInsurance;
        private MNCService serviceMNC;
        private MNCRepo mncRepo;
        public InsuranceController(IConfiguration configuration, IHostingEnvironment environment)
        {
            serviceInsurance = new InsuranceService(configuration, environment);
            fn = new FunctionString(Tools.ConnectionString(configuration));
            _environment = environment;
            serviceMNC = new MNCService(configuration);
            mncRepo = new MNCRepo(Tools.ConnectionString(configuration));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost("UploadInsurancePolicy")]
        //[APIAuthorizeAttribute]
        [ProducesResponseType(typeof(OutputVendor), 200)]
        //public void UploadFile([FromForm]UploadFile uploadFile)
        public async Task<OutputVendor> UploadInsurancePolicy([FromForm] UploadFileInsurance uploadFile)
        {
            var po = new OutputVendor();
            try
            {
                VendorInsuranceLog dataLog = new VendorInsuranceLog();
                
                var paramString = JsonConvert.SerializeObject(uploadFile);

                var folderName = Path.Combine("Insurance");
                var pathToSave = Path.Combine(_environment.WebRootPath, folderName);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                Dictionary<string, string> data = new Dictionary<string, string>();
                //foreach (var file in uploadFile.images)
                //{
                var file = uploadFile.file_upload;
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                

                string fileType = Path.GetExtension(fileName).ToLower();
                
                var nameunix = DateTime.Now.Ticks.ToString() + fileType;
                //var fullPath = Path.Combine(pathToSave, linkAttachment.ToString());
                var fullPath = Path.Combine(pathToSave, nameunix);
                var dbPath = Path.Combine(folderName, nameunix);

                #region insert log error atau gk
                dataLog.vendor_token = fn.DecryptString(uploadFile.vendor_token);
                dataLog.ip_address = Tools.GetIpAddress();
                dataLog.order_no = uploadFile.order_no;
                dataLog.user_agent = Tools.GetUserAgent();
                dataLog.path_file = dbPath;
                dataLog.file_name = fileName;
                dataLog.param_string = paramString;
                dataLog.user_input = dataLog.vendor_token;
                dataLog.insurance_policy_no = uploadFile.insurance_policy_no.Trim();
                serviceInsurance.InsertLog(dataLog);
                #endregion
                serviceInsurance.ValidasiData(uploadFile);
                if (fileType != ".pdf")
                {
                    throw new Exception("Invalid FileUpload type.");
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                serviceInsurance.UpdateOrder(file.FileName, dbPath, uploadFile.order_no, uploadFile.insurance_policy_no);
             
                po.ResponseMsg = "success";

                

            }
            catch (Exception ex)
            {
                po.ResponseCode = 0;
                po.ResponseMsg = ex.Message;
            }
            
            return po;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VendorID"></param>
        /// <returns></returns>
        [HttpGet("HasVendorID")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<Output> HasVendorID(string VendorID)
        {
            var po = new Output();
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("VendorToken", EncryptionLibrary.EncryptText(VendorID));
                po.Data = data;
            }
            catch (Exception ex)
            {
                po.Error = true;
                po.Message = ex.Message;
            }

            return po;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [APIAuthorizeAttribute]
        [HttpPost("Submission")]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> DoV1([FromBody] ParameterMNC Model)
        {
            var _result = new Output();
            try
            {
                _result = await serviceMNC.Submission(Model);
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
