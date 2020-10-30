using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class UserFCMRepo
    {
        private string connectionString;
        private FunctionString fn;
        public UserFCMRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public bool SaveUserFCM(int PortfolioId, string UserId, string FCMToken, string Token)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    FCMToken = string.IsNullOrEmpty(FCMToken) ? DateTime.Now.Ticks.ToString() : FCMToken;
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", UserId);
                    Parameters.Add("p_fcm_token", FCMToken);
                    Parameters.Add("p_token", Token);
                    Parameters.Add("p_user_input", UserId);
                    var dd = conn.Query<dynamic>("fss_chat_user_fcm_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteUserFCM(string token)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"
                    DELETE FROM ss_chat_user_fcm
                    WHERE ss_portfolio_id = @ss_portfolio_id
                    AND user_id = @user_id
                    AND token = @token;

";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_portfolio_id = Tools.PortfolioId, user_id = Tools.UserId, token = token });
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
        public List<UserFCM> GetList(int ChatID, string UserFrom)
        {
            List<UserFCM> op = new List<UserFCM>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {

                    //sParameter = fn.FormatWhere(sParameter);
                    string sQuery = @"
                    select 
	                    x.fcm_token,	x.token,
                        x.user_id,		y.ss_portfolio_id
                    from ss_chat_user_fcm x 
                    join ss_chat_h_user y 
                      ON x.ss_portfolio_id = y.ss_portfolio_id
                      and x.user_id = y.user_id
                    where y.ss_chat_h_id = @ss_chat_h_id
	                    and y.ss_portfolio_id= @ss_portfolio_id
                        and x.user_id <> @user_id              
";
                    conn.Open();
                    op = conn.Query<UserFCM>(sQuery, new { ss_chat_h_id = ChatID, ss_portfolio_id = Tools.PortfolioId, user_id = UserFrom }).ToList();
                }
                catch (Exception ex)
                {
                    //op = Helper.Error(ex);
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return op;
        }

        public ChatNotif GetSumChatNotif(int portfolio_id, string user_id)
        {
            ChatNotif _result = new ChatNotif();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("p_user_id", user_id);
                    Parameters.Add("p_ss_portfolio_id", portfolio_id, dbType: DbType.Int32);

                    var dd = conn.Query<ChatNotif>("fss_get_sum_notif_chat", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    _result = dd;
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
            return _result;
        }
    }
}
