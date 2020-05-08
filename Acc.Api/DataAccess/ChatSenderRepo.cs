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
    public class ChatSenderRepo
    {
        private string connectionString;
        public ChatSenderRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public ChatSender GetDataHeader(int key)
        {
            ChatSender t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = "SELECT   subject,  user_id_from,  user_id_to,  doc_no,  user_input FROM public.ss_chat_h  WHERE ss_chat_h_id = @ss_chat_h_id";
                try
                {
                    conn.Open();
                    t = conn.Query<ChatSender>(strQuery, new { ss_chat_h_id = key }).SingleOrDefault();
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

        public object SaveHeader(ChatSender Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                object _result = null;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Convert.ToInt32(Model.portfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_ss_subportfolio_id", Convert.ToInt32(Model.subportfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_subject", Model.subject);
                    Parameters.Add("p_user_id_from", Model.user_id_from);
                    Parameters.Add("p_user_id_to", Model.user_id_to);
                    Parameters.Add("p_doc_date", DateTime.Now, dbType: DbType.DateTime);
                    Parameters.Add("p_doc_type", Model.doc_type);
                    Parameters.Add("p_doc_no", Model.doc_no);
                    Parameters.Add("p_user_input", Model.user_input);
                    _result = conn.Query<dynamic>("fss_chat_h_i", Parameters, commandType: CommandType.StoredProcedure).ToList();

                    //result = conn.Execute(sqlQuery, new { ss_menu_id = key });

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

        public List<GetChat> GetAllChat(int HeaderId,int curr_page)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                List<GetChat> _result = new List<GetChat>();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_header_id", HeaderId, dbType: DbType.Int32);
                    Parameters.Add("p_page", curr_page, dbType: DbType.Int32);
                    _result = conn.Query<GetChat>("fss_chat_d_getallchat", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public List<GetChat> GetChat(int HeaderId, string user_id)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                List<GetChat> _result = new List<GetChat>();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_header_id", HeaderId, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", user_id);
                    _result = conn.Query<GetChat>("fss_chat_d_getchat", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool UpdateStatusChat(GetChat Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                bool _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_d_id", Model.ss_chat_d_id, dbType: DbType.Int32);
                    var dd = conn.Query<dynamic>("fss_chat_d_u_chatstatus", Parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        public bool SendChat(ChatDetail Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_h_id", Model.ss_chat_h_id, dbType: DbType.Int32);
                    Parameters.Add("p_chat_text", Model.chat_text);
                    Parameters.Add("p_chat_date", Model.chat_date, dbType: DbType.DateTime);
                    Parameters.Add("p_user_id_from", Model.user_id_from);
                    Parameters.Add("p_user_id_to", Model.user_id_to);
                    Parameters.Add("p_user_input", Model.user_input);
                    var dd = conn.Query<dynamic>("fss_chat_d_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
