using Acc.Api.Helper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class MNCRepo
    {
        private string connectionString;
        public MNCRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public bool UpdateOpOrder(int SsPortfolio,string OrderNo, string InsurancePolicyNo)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"    update op_order
                                set insurance_policy_no = @insurance_policy_no
                                where order_no = @order_no
                                AND ss_portfolio_id = @ss_portfolio_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_portfolio_id = SsPortfolio, order_no = OrderNo, insurance_policy_no = InsurancePolicyNo });
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return (result > 0);
        }
        public bool InsertLog(string SsPortfolioId,string OrderNo,string DataPost,string DataResponse)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", SsPortfolioId);
                    Parameters.Add("p_order_no", OrderNo);
                    Parameters.Add("p_data_post", DataPost);
                    Parameters.Add("p_data_response", DataResponse);                    
                    Parameters.Add("p_user_input", "API");
                    var dd = conn.Query<dynamic>("fss_post_insurance_log_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
                    _result = true;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }

                return _result;
            }
        }
    }
}
