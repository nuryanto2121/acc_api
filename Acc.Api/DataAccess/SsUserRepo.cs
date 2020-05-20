using Acc.Api.Enum;
using Acc.Api.Helper;
using Acc.Api.Interface;
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
    public class SsUserRepo : IRepository<SsUser, int>
    {
        private string connectionString;
        private FunctionString fn;
        public SsUserRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public bool Delete(int key)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_user WHERE ss_user_id = @ss_user_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_user_id = key });
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

        public bool Delete(int key, int timestamp)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_user WHERE ss_user_id = @ss_user_id and xmin::text::integer = @timestamp";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_user_id = key, timestamp = timestamp });
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

        public SsUser GetById(int key, int lastupdatestamp)
        {
            SsUser t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT  *
                        FROM public.vss_user_by  WHERE ss_user_id = @ss_user_id and lastupdatestamp = @lastupdatestamp";
                try
                {
                    conn.Open();
                    t = conn.Query<SsUser>(strQuery, new { ss_user_id = key, lastupdatestamp = lastupdatestamp }).SingleOrDefault();
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

        public List<SsUser> GetList()
        {
            throw new NotImplementedException();
        }

        public List<SsUser> GetList(int pageSize, int currentPage, string sortName, string sortOrder, string Parameter)
        {
            throw new NotImplementedException();
        }

        public bool Save(SsUser domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO public.ss_user (
                                        user_id,                ss_group_id,
                                        user_name,              password,
                                        email,                  user_level,
                                        expired_date,           is_inactive,
                                        job_title,              hand_phone,
                                        last_change_password,   default_language,
                                        user_input,             user_edit,
                                        portfolio_id,           subportfolio_id,
                                        time_input,             time_edit,
                                        file_name,              path_file,
                                        address,                notes
                                )
                                VALUES (
                                      @user_id,                                      @group_id,
                                      @user_name,                                      @password,
                                      @email,                                      @user_level,
                                      @expired_date,                                      @is_inactive,
                                      @job_title,                                      @hand_phone,
                                      @last_change_password,                                      @default_language,
                                      @user_input,                                      @user_edit,
                                      @portfolio_id,                                      @subportfolio_id,
                                      @time_input,                                      @time_edit,
                                      @file_name,                                      @path_file,
                                      @address,                                         @notes
                                );";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, domain);
                    result = true;
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
            return result;
        }

        public object SelectScalar(SQL.Function.Aggregate function, string column)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> QueryList(string tableName, int iStart, int iPageSize, string sSortField, string sParameter)
        {
            List<dynamic> op = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    int iOffset = (iStart * iPageSize) - iPageSize;
                    sParameter = string.IsNullOrEmpty(sParameter) ? sParameter : sParameter.ToUpper().Contains("WHERE") ? sParameter : "WHERE " + sParameter;
                    //sParameter = fn.FormatWhere(sParameter);
                    StringBuilder sQuery = new StringBuilder();
                    sQuery.AppendFormat(" WITH result_set AS ");
                    sQuery.AppendFormat(" ( ");
                    sQuery.AppendFormat("SELECT  ");
                    sQuery.AppendFormat(" row_number() OVER ({0}) as no,  ", sSortField);
                    sQuery.AppendFormat(" * ");
                    sQuery.AppendFormat(" FROM {0} ", tableName);
                    sQuery.AppendFormat(" {0} ", sParameter);
                    sQuery.AppendFormat(" ) ");
                    sQuery.AppendFormat(" SELECT * FROM  result_set ", sSortField);
                    sQuery.AppendFormat(" LIMIT {0} ", iPageSize);
                    sQuery.AppendFormat(" OFFSET {0} ;", iOffset);
                    conn.Open();
                    op = conn.Query<dynamic>(sQuery.ToString()).ToList();
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

        public bool Update(SsUser domain)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                //string sqlQuery = "UPDATE public.ss_user SET title = @title,url = @menu_url,parent_menu_id = @parent_menu_id,parent_menu_title = @icon_class = @icon_class,path = @path,order_seq = @order_seq,user_edit = @user_edit,time_edit = @time_edit WHERE ss_user_id = @ss_user_id";
                //AND xmin::text::integer = @lastupdatestamp";
                string sqlQuery = @"UPDATE public.ss_user 
                                    SET 
                                      user_id = @user_id,
                                      ss_group_id = @group_id,
                                      user_name = @user_name,
                                      password = @password,
                                      email = @email,
                                      user_level = @user_level,
                                      job_title = @job_title,
                                      hand_phone = @hand_phone,
                                      last_change_password = @last_change_password,
                                      default_language = @default_language,
                                      user_edit = @user_edit,
                                      time_edit = @time_edit,
                                      file_name = @file_name,
                                      path_file = @path_file,
                                      address = @address,                
                                      notes = @notes
                                    WHERE 
                                      ss_user_id = @ss_user_id
                                        and  xmin::text::integer = @lastupdatestamp
                                    ; ";

                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, domain);
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
