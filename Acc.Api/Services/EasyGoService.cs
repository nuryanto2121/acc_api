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
    public class EasyGoService
    {
        IConfiguration config;
        private EasyGoRepo easyGoRepo;
        public EasyGoService(IConfiguration Configuration)
        {
            config = Configuration;
            easyGoRepo = new EasyGoRepo(Tools.ConnectionString(Configuration));
        }

        public async Task<Output> DOV1(EasyGoAdd Model)
        {
            Output _result = new Output();
            try
            {
                ResponseDO Resp = new ResponseDO();
                EasyGoDO ParamDO = new EasyGoDO();
                var Tokens = easyGoRepo.GetToken(Tools.PortfolioId);

                ParamDO = Model.EasyGoDo;

                //ParamDO.GeoAsal.ForEach()
                ParamDO.GeoAsal.ForEach(delegate (GeoAsal dt)
                {
                    if (string.IsNullOrEmpty(dt.Lat))
                    {
                        dt.Lat = "-6.182101";
                    }
                    if (string.IsNullOrEmpty(dt.Lon))
                    {
                        dt.Lon = "106.914125";
                    }
                });

                ParamDO.GeoTujuan.ForEach(delegate (GeoTujuan dt)
                {
                    if (string.IsNullOrEmpty(dt.Lat))
                    {
                        dt.Lat = "-7.252438";
                    }
                    if (string.IsNullOrEmpty(dt.Lon))
                    {
                        dt.Lon = "112.750270";
                    }
                });

                if (Tokens == null || string.IsNullOrEmpty(Tokens.gps_token))
                {
                    throw new Exception("Please contact your Administartor. (Setting Token EasyGo)");
                }

                var handler = new HttpClientHandler()
                {
                    Proxy = HttpWebRequest.GetSystemWebProxy(),
                    UseDefaultCredentials = true
                };


                using (var client = new HttpClient(handler))
                {
                    var url = "https://vtsapi.easygo-gps.co.id/api/do/AddOrUpdateDOV1ByLatLon";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Token", Tokens.gps_token);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(ParamDO), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync(url, content))
                    {

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Resp = JsonConvert.DeserializeObject<ResponseDO>(apiResponse);
                        _result.Data = Resp;
                        if (Resp.ResponseCode == 1)
                        {
                            easyGoRepo.UpdateOpOrder(Model.OpOrderID, Resp.Data.DoID);
                            _result.Data = Resp;
                        }
                        //else
                        //{
                        //    _result.Error = true;
                        //    _result.Message = Resp.ResponseMessage;
                        //    _result.Status = 501;
                        //    _result.Data = Resp;
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                _result = Tools.Error(ex);
            }
            return _result;
        }

        public async Task<Output> CloaseDoV1(int ID, EasyGoCloasParam Model)
        {
            Output _result = new Output();
            try
            {
                ResponseDO Resp = new ResponseDO();
                EasyGoClose ParamEasyGo = new EasyGoClose();
                var Tokens = easyGoRepo.GetToken(Tools.PortfolioId);

                ParamEasyGo = Model.EasyGoCloseDo;
                if (Tokens == null || string.IsNullOrEmpty(Tokens.gps_token))
                {
                    throw new Exception("Please contact your Administartor. (Setting Token EasyGo)");
                }

                var handler = new HttpClientHandler()
                {
                    Proxy = HttpWebRequest.GetSystemWebProxy(),
                    UseDefaultCredentials = true
                };


                using (var client = new HttpClient(handler))
                {
                    var url = string.Format("https://vtsapi.easygo-gps.co.id/api/do/closeDOV1/{0}", ID);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Token", Tokens.gps_token);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(ParamEasyGo), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync(url, content))
                    {

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Resp = JsonConvert.DeserializeObject<ResponseDO>(apiResponse);
                        _result.Data = Resp;
                        //if (Resp.ResponseCode != 1)
                        //{
                        //    _result.Error = true;
                        //    _result.Message = Resp.ResponseMessage;
                        //    _result.Status = 200;
                        //    _result.Data = Resp;
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                _result = Tools.Error(ex);
            }
            return _result;
        }

        public async Task<Output> LastPosition(EasyGoLastPositionParam Model)
        {
            Output _result = new Output();
            try
            {
                ResponsePosition Resp = new ResponsePosition();
                EasyGoLastPosition ParamEasyGo = new EasyGoLastPosition();
                var Tokens = easyGoRepo.GetToken(Tools.PortfolioId);

                ParamEasyGo = Model.EasyGoLastPosition;
                if (Tokens == null || string.IsNullOrEmpty(Tokens.gps_token))
                {
                    throw new Exception("Please contact your Administartor. (Setting Token EasyGo)");
                }

                var handler = new HttpClientHandler()
                {
                    Proxy = HttpWebRequest.GetSystemWebProxy(),
                    UseDefaultCredentials = true
                };


                using (var client = new HttpClient(handler))
                {
                    var url = "https://vtsapi.easygo-gps.co.id/api/report/lastposition";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Token", Tokens.gps_token);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(ParamEasyGo), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync(url, content))
                    {

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Resp = JsonConvert.DeserializeObject<ResponsePosition>(apiResponse);
                        _result.Data = Resp;
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
