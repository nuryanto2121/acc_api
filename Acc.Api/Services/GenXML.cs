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
        string SpInvoice = "";
        private string connectionString;
        public GenXML(IConfiguration Configuration, IHostingEnvironment environment)
        {
            config = Configuration;
            _environment = environment;
            PathCOA = string.Empty;
            connectionString = Tools.ConnectionString(Configuration);
        }

        private string xmlCoa()
        {
            return @"
        {
               'NMEXML': {
                  '@EximID': '3',
                  '@BranchCode': '841463763',
                  '@ACCOUNTANTCOPYID': '',
                  'TRANSACTIONS': {
                     '@OnError': 'continue',
                     'OTHERPAYMENT': [
                        {
                           '@operation': 'Add',
                           '@REQUESTID': '1',
                           'TRANSACTIONID': '1',
                           'ACCOUNTLINE': {
                              '@operation': 'Add',
                              'KeyID': '1',
                              'GLACCOUNT': '5100-001',
                              'GLAMOUNT': '725000',
                              'DESCRIPTION': [],
                              'RATE': '1',
                              'PRIMEAMOUNT': '725000',
                              'TXDATE': [],
                              'POSTED': [],
                              'CURRENCYNAME': []
                           },
                           'JVNUMBER': 'CA-2101771',
                           'TRANSDATE': '2021-03-01',
                           'SOURCE': 'GL',
                           'TRANSTYPE': 'other payment',
                           'TRANSDESCRIPTION': 'B 9442 TEH\nTAMRIN',
                           'JVAMOUNT': '725000',
                           'CHEQUENO': [],
                           'PAYEE': 'PT. Bintang Baru Raya',
                           'VOIDCHEQUE': [],
                           'GLACCOUNT': '1112-001',
                           'RATE': '1'
                        },
                        {
                           '@operation': 'Add',
                           '@REQUESTID': '1',
                           'TRANSACTIONID': '1',
                           'ACCOUNTLINE': [
                              {
                                 '@operation': 'Add',
                                 'KeyID': '1',
                                 'GLACCOUNT': '5100-001',
                                 'GLAMOUNT': '545000',
                                 'DESCRIPTION': [],
                                 'RATE': '1',
                                 'PRIMEAMOUNT': '545000',
                                 'TXDATE': [],
                                 'POSTED': [],
                                 'CURRENCYNAME': []
                              },
                              {
                                 '@operation': 'Add',
                                 'KeyID': '2',
                                 'GLACCOUNT': '5100-001',
                                 'GLAMOUNT': '100000',
                                 'DESCRIPTION': [],
                                 'RATE': '1',
                                 'PRIMEAMOUNT': '100000',
                                 'TXDATE': [],
                                 'POSTED': [],
                                 'CURRENCYNAME': []
                              },
                              {
                                 '@operation': 'Add',
                                 'KeyID': '3',
                                 'GLACCOUNT': '5100-001',
                                 'GLAMOUNT': '200000',
                                 'DESCRIPTION': [],
                                 'RATE': '1',
                                 'PRIMEAMOUNT': '200000',
                                 'TXDATE': [],
                                 'POSTED': [],
                                 'CURRENCYNAME': []
                              }
                           ],
                           'JVNUMBER': 'CA-2101907',
                           'TRANSDATE': '2021-03-01',
                           'SOURCE': 'GL',
                           'TRANSTYPE': 'other payment',
                           'TRANSDESCRIPTION': 'B 9005 TEU\nJAMADI',
                           'JVAMOUNT': '845000',
                           'CHEQUENO': [],
                           'PAYEE': 'PT. Bintang Baru Raya',
                           'VOIDCHEQUE': [],
                           'GLACCOUNT': '1112-001',
                           'RATE': '1'
                        }
                     ]
                  }
               }
            }

        ";
        }

        public string GenCOA(ParamCoaXml Model,string FolderPath, string PathRoot)
        {
            try
            {
                string XMLCoa = this.xmlCoa(Convert.ToInt32(Model.SsPortfolioId), Model.UserInput);
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

        private string xmlCoa(int SsPortfolioId,string UserInput)
        {
            string resultXml = string.Empty;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {                
                try
                {
                    DynamicParameters spParam = new DynamicParameters();
                    spParam.Add("p_ss_portfolio_id", SsPortfolioId, dbType: DbType.Int32);
                    spParam.Add("p_user_input", UserInput);
                    var datas = conn.Query(SpCoa, spParam, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
