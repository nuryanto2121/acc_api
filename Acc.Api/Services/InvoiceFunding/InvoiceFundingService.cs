using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class InvoiceFundingService
    {
        string connectionString = string.Empty;
        private FunctionString fn;
        private SsPortfolioRepo ssPortfolioRepo;
        public InvoiceFundingService(IConfiguration configuration)
        {
            connectionString = Tools.ConnectionString(configuration);
            fn = new FunctionString(connectionString);
            ssPortfolioRepo = new SsPortfolioRepo(connectionString);
        }

        public async Task<Output> GetInvoice(ParamInvoiceFunding Param)
        {
            var result = new Output();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    SsPortfolio DataPortfolio = ssPortfolioRepo.GetById(Param.SsPortfolioID, 0);
                    string Token = string.Format("{0}{1}{2}", DataPortfolio.merchant_id_funding, Param.InvoiceNo, Param.SsPortfolioID);

                    if (Param.Token != fn.sha256_hash(Token))
                    {
                        //throw new Exception("Invalid Token.");
                        result.Data = null;
                        result.Message = "Invalid Token.";
                        result.Error = true;
                        result.Status = 404;
                        return result;
                    }
                    conn.Open();
                    DynamicParameters spParam = new DynamicParameters();
                    string SpName = "fst_invoice_for_funding_s";
                    spParam.Add("p_ss_portfolio_id", Param.SsPortfolioID, dbType: DbType.Int32);
                    spParam.Add("p_invoice_no", Param.InvoiceNo);
                    var datas = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var res = JObject.FromObject(datas);
                    if (res[SpName] != null)
                    {
                        var dd = JsonConvert.DeserializeObject(res[SpName].ToString());
                        result.Data = dd;
                    }
                    else
                    {
                        result.Data = null;
                        result.Message = "Data Not Found.";
                        result.Error = true;
                        result.Status = 404;
                        //throw new Exception("Data Not Found.");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
    }
}
