using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class MNCService
    {
        IConfiguration config;
        private FunctionString fn;
        private MNCRepo mncRepo;
        private InsuranceRepo repoInsurance;
        public MNCService(IConfiguration Configuration)
        {
            config = Configuration;
            fn = new FunctionString(Tools.ConnectionString(Configuration));
            mncRepo = new MNCRepo(Tools.ConnectionString(Configuration));
            repoInsurance = new InsuranceRepo(Tools.ConnectionString(Configuration));
        }

        public async Task<Output> Submission(ParameterMNC ModelParam)
        {
            Output _result = new Output();
            try
            {
                ParameterPostMNC Model = new ParameterPostMNC();
                string MerchantID = string.Empty;
                string SecretKey = string.Empty;
                string url = string.Empty;
                //int PortfolioId = 0;// (ModelParam.SsPortfolioID)
                ModelParam.SsPortfolioID = Tools.DecryptString(ModelParam.SsPortfolioID);

                var DataVendorInsurance = repoInsurance.GetVendorInsurance(ModelParam.InsuranceCode);
                var isProd = repoInsurance.IsProdInsurance(Convert.ToInt32(ModelParam.SsPortfolioID));
                MerchantID = isProd ? DataVendorInsurance.merchant_id : DataVendorInsurance.merchant_id_uat;
                SecretKey = isProd ? DataVendorInsurance.secret_key : DataVendorInsurance.secret_key_uat;
                url = isProd ? DataVendorInsurance.url : DataVendorInsurance.url_uat;

                Model = ModelParam;

                Model.MerchantToken = fn.sha256_hash(MerchantID + Model.Sot + Model.VehiclePlatNo + SecretKey);

                var handler = new HttpClientHandler()
                {
                    Proxy = HttpWebRequest.GetSystemWebProxy(),
                    UseDefaultCredentials = true
                };


                using (var client = new HttpClient(handler))
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(Model), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync(url, content))
                    {
                        
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var Resp = JsonConvert.DeserializeObject<RespsoneMNC>(apiResponse);
                        _result.Data = Resp;
                        string ss = JsonConvert.SerializeObject(Model);
                        mncRepo.InsertLog(ModelParam.SsPortfolioID, Model.Sot, ss, apiResponse);
                        if (Resp.ResultCd== "0000")
                        {
                            mncRepo.UpdateOpOrder(Convert.ToInt32(ModelParam.SsPortfolioID), Model.Sot, Resp.CertificateNo);
                        }
                        
                        
                    }
                }

            }
            catch (Exception ex)
            {
                _result = Tools.Error(ex);
            }
            return _result;
        }
    }
}
