﻿using Acc.Api.Helper;
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

        public ChatHeader GetDataHeader(int key)
        {
            ChatHeader t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT ss_chat_h_id,  ss_portfolio_id,  ss_subportfolio_id,  subject,
                                    user_id_to,  doc_date,  doc_type,  doc_no,   user_id_from,
                                    row_id,  option_url,  line_no,  url_view_detail FROM public.ss_chat_h  WHERE ss_chat_h_id = @ss_chat_h_id";
                try
                {
                    conn.Open();
                    t = conn.Query<ChatHeader>(strQuery, new { ss_chat_h_id = key }).SingleOrDefault();
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

        public List<ChatListUser> GetDataUserList(int HeaderId,int PortfolioId)
        {
            List<ChatListUser> t = new List<ChatListUser>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT a.ss_chat_h_id,  a.ss_portfolio_id, a.user_id, a.is_admin  ,b.user_name
                                    FROM public.ss_chat_h_user a
                                    join ss_user b 
                                        on b.user_id = a.user_id  
                                        AND a.ss_portfolio_id = b.portfolio_id
                                    WHERE a.ss_chat_h_id = @ss_chat_h_id AND a.ss_portfolio_id = @ss_portfolio_id";
                try
                {
                    conn.Open();
                    t = conn.Query<ChatListUser>(strQuery, new { ss_chat_h_id = HeaderId, ss_portfolio_id= PortfolioId }).ToList();
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
        public object RemoveUser(int ID,string UserRemove)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                object _result = null;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_id", ID, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", UserRemove);
                    _result = conn.Query<dynamic>("fss_chat_remove_user", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public object RemoveUserAll(int ID)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                object _result = null;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_id", ID, dbType: DbType.Int32);
                    _result = conn.Query<dynamic>("fss_chat_remove_user_a", Parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public object AddUser(int ID, string AddUser,bool IsAdmin = false)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                object _result = null;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_h_id", ID, dbType: DbType.Int32);
                    Parameters.Add("p_ss_portfolio_id", Tools.PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", AddUser);
                    Parameters.Add("p_is_admin", IsAdmin);
                    Parameters.Add("p_user_input", Tools.UserId);
                    _result = conn.Query<dynamic>("fss_chat_h_user_m_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
                    Parameters.Add("p_option_url", Model.option_url);
                    Parameters.Add("p_line_no", Model.line_no, dbType: DbType.Int32);
                    Parameters.Add("p_url_view_detail", Model.url_view_detail);
                    Parameters.Add("p_row_id", Model.row_id, dbType: DbType.Int32);
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
        public object DeleteChat(int ID)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                object _result = null;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_row_id", ID, dbType: DbType.Int32);
                    _result = conn.Query<dynamic>("fss_chat_h_d", Parameters, commandType: CommandType.StoredProcedure).ToList();

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
        public List<GetChat> GetAllChat(int HeaderId, int curr_page)
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
        public bool UpdateUserHeader(int Key, string UserIdTo)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                bool _result = false;
                try
                {
                    string sqlQuery = @"UPDATE public.ss_chat_h 
                                    SET 
                                      user_id_to = @user_id_to
                                    WHERE 
                                      ss_chat_h_id = @ss_chat_h_id
                                    ; ";
                    conn.Open();
                    conn.Execute(sqlQuery, new { user_id_to = UserIdTo, ss_chat_h_id = Key });
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
        public RowID SendChat(ChatDetail Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_h_id", Model.ss_chat_h_id, dbType: DbType.Int32);
                    Parameters.Add("p_chat_text", Model.chat_text);
                    Parameters.Add("p_chat_date", Model.chat_date, dbType: DbType.DateTime);
                    Parameters.Add("p_user_id_from", Model.user_id_from);
                    Parameters.Add("p_user_id_to", Model.user_id_to);
                    Parameters.Add("p_is_file", Model.is_file);
                    Parameters.Add("p_ss_chat_attachment_id", Model.ss_chat_attachment_id);                    
                    Parameters.Add("p_user_input", Model.user_input);
                    _result = conn.Query<RowID>("fss_chat_d_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    //_result = true;
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
        public RowID SendAttachment(ChatAttachment Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_h_id", Model.ss_chat_h_id, dbType: DbType.Int32);
                    Parameters.Add("p_ss_portfolio_id", Model.ss_portfolio_id);
                    Parameters.Add("p_file_name", Model.file_name);
                    Parameters.Add("p_file_type", Model.file_type);
                    Parameters.Add("p_path_file", Model.path_file);
                    Parameters.Add("p_user_input", Tools.UserId);
                    _result =  conn.Query<RowID>("fss_chat_attachment_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    //_result = true;
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
        public bool SendChatRead(ChatDetail Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_d_id", Model.ss_chat_d_id, dbType: DbType.Int32);
                    Parameters.Add("p_user_id_from", Model.user_id_from);
                    Parameters.Add("p_user_id_to", Model.user_id_to);
                    Parameters.Add("p_is_read", false, dbType: DbType.Boolean);
                    Parameters.Add("p_user_input", Model.user_input);
                    var dd = conn.Query<dynamic>("fss_chat_d_read_id_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool UpdateStatusChatRead(int Key,string From,string To)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_chat_d_id", Key, dbType: DbType.Int32);
                    Parameters.Add("p_user_id_from", From);
                    Parameters.Add("p_user_id_to", To);                    
                    var dd = conn.Query<dynamic>("fss_chat_d_read_id_u_status", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
