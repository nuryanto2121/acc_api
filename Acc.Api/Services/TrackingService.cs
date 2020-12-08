using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class TrackingService
    {
        private FunctionString fn;
        private TrackingRepo trackingRepo;
        public TrackingService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            trackingRepo = new TrackingRepo(Tools.ConnectionString(configuration));
        }

        public Output GetDataTracking(string OrderNo,string captcha)
        {
            Output _result = new Output();
            try
            {
               
                TrackingAccessLog dataLog = trackingRepo.GetDataLog(Tools.GetIpAddress(), captcha, Tools.GetUserAgent());
                if (dataLog == null || string.IsNullOrEmpty(dataLog.ip_address))
                {
                    trackingRepo.DeleteChaptcha(Tools.GetIpAddress(), Tools.GetUserAgent());

                    JObject Captcha = new JObject();
                    string dtCaptcha = EncryptionLibrary.KeyGenerator.GetUniqueKey(6);

                    trackingRepo.SaveLog(dtCaptcha, Tools.GetUserAgent());

                    Captcha.Add("Captcha", dtCaptcha);
                    _result.Data = Captcha;
                    _result.Message = "Invalid Captcha";
                    _result.Error = true;
                    _result.Status = 500;
                    return _result;
                    //throw new Exception("Invalid Captcha");
                }

                RowID dataRowID = trackingRepo.GetRowID(OrderNo);
                if (dataRowID == null || dataRowID.row_id == 0)
                {
                    throw new Exception("Invalid Order No.");
                }
                _result.Data = trackingRepo.GetDataTracking(dataRowID.row_id);

                trackingRepo.UpdateChaptcha(Tools.GetIpAddress(), captcha, Tools.GetUserAgent());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GenCaptcha()
        {
            Output _result = new Output();
            try
            {                
                JObject Captcha = new JObject();
                string dtCaptcha = EncryptionLibrary.KeyGenerator.GetUniqueKey(6);
                Captcha.Add("Captcha", dtCaptcha);

                trackingRepo.SaveLog(dtCaptcha, Tools.GetUserAgent());

                _result.Data = Captcha;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
