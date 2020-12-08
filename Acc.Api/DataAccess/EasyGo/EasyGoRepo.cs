using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class EasyGoRepo
    {
        private string connectionString;
        public EasyGoRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public TokenEasyGO GetToken(int ID)
        {
            TokenEasyGO t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"select gps_token,gps_map_token from ss_portfolio where ss_portfolio_id = @ss_portfolio_id";
                try
                {
                    conn.Open();
                    t = conn.Query<TokenEasyGO>(strQuery, new { ss_portfolio_id = ID }).SingleOrDefault();
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

            return t;
        }
        public bool UpdateOpOrder(int OpOrderId,int GpsDoId)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"    update op_order
                                set gps_do_id = @gps_do_id
                                where op_order_id = @op_order_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { op_order_id = OpOrderId, gps_do_id = GpsDoId });
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
    }
}
