using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Acc.Api.Services
{
    public class GenXML
    {
        IConfiguration config;
        private string PathCOA;
        private IHostingEnvironment _environment;
        string SpCoa = "f_st_export_oca_s";
        string SpInvoice = "f_st_export_invoice_s";
        private string connectionString;
        public GenXML(IConfiguration Configuration, IHostingEnvironment environment)
        {
            config = Configuration;
            _environment = environment;
            PathCOA = string.Empty;
            connectionString = Tools.ConnectionString(Configuration);
        }


        public string GenCOA(ParamCoaXml Model, string FolderPath, string PathRoot)
        {
            try
            {
                string XMLCoa = this.xmlData(Convert.ToInt32(Model.SsPortfolioId), Model.UserInput);
                XNode node = JsonConvert.DeserializeXNode(XMLCoa, "NMEXML");
                string XMLString = node.ToString();
                PathCOA = string.Format("ACCELOGBBR_vendorxml_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmss"));
                var PathComponent = Path.Combine(FolderPath, PathCOA);

                string[] XMLdata = XMLString.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );
                Tools.writeFile(XMLdata, PathComponent);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PathCOA;
        }
        public string GenInvoice(ParamCoaXml Model, string FolderPath, string PathRoot)
        {
            try
            {
                string XMLCoa = this.xmlData(Convert.ToInt32(Model.SsPortfolioId), Model.UserInput, false);
                XNode node = JsonConvert.DeserializeXNode(XMLCoa, "NMEXML");
                string XMLString = node.ToString();
                PathCOA = string.Format("ACCELOGBBR_salesinv_xml_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmss"));
                var PathComponent = Path.Combine(FolderPath, PathCOA);

                string[] XMLdata = XMLString.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );
                Tools.writeFile(XMLdata, PathComponent);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PathCOA;
        }

        private string xmlData(int SsPortfolioId, string UserInput, bool isCoa = true)
        {
            string resultXml = string.Empty;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string SPName = isCoa ? SpCoa : SpInvoice;
                    DynamicParameters spParam = new DynamicParameters();
                    spParam.Add("p_ss_portfolio_id", SsPortfolioId, dbType: DbType.Int32);
                    spParam.Add("p_user_input", UserInput);
                    var datas = conn.Query(SPName, spParam, commandTimeout: 30000, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var res = JObject.FromObject(datas);
                    if (res["result"] != null)
                    {
                        resultXml = res["result"].ToString();
                    }
                    else
                    {
                        if (res[SpCoa] != null)
                        {
                            resultXml = res[SpCoa].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return resultXml;
        }

    }
}
