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
        public object GetMenuJson(int? portfolio_id, string user_id,string group_access)
        {
            object _result = new object();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("p_user_id", user_id);

                    if (portfolio_id == 0)
                    {
                        Parameters.Add("p_ss_portfolio_id", null, dbType: DbType.Int32);
                    }
                    else
                    {
                        Parameters.Add("p_ss_portfolio_id", portfolio_id, dbType: DbType.Int32);
                    }

                    Parameters.Add("p_group_access", group_access);

                    var dd = conn.Query<dynamic>("get_menu_json_user", Parameters, commandType: CommandType.StoredProcedure);
                    //var dd = conn.Query<dynamic>("get_menu_json", Parameters, commandType: CommandType.StoredProcedure);
                    _result = dd;
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
            }
            return _result;
        }
        public bool SaveDetail(SsMenuUser Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Model.ss_portfolio_id, dbType: DbType.Int32);
                    Parameters.Add("p_ss_menu_id", Model.ss_menu_id);
                    Parameters.Add("p_user_id", Model.user_id);
                    Parameters.Add("p_add_status", Model.add_status, dbType: DbType.Boolean);
                    Parameters.Add("p_edit_status", Model.edit_status, dbType: DbType.Boolean);
                    Parameters.Add("p_delete_status", Model.delete_status, dbType: DbType.Boolean);
                    Parameters.Add("p_view_status", Model.view_status, dbType: DbType.Boolean);
                    Parameters.Add("p_post_status", Model.post_status, dbType: DbType.Boolean);
                    Parameters.Add("p_user_input", Model.user_input);
                    var dd = conn.Query<dynamic>("fss_menu_user_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        
        public bool DeleteButtonUser(int PortfolioId, string UserId)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);                   
                    Parameters.Add("p_user_id", UserId);
                    var dd = conn.Query<dynamic>("fss_user_menu_button_access_d", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteDashboardUser(int PortfolioId, string UserId)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", UserId);
                    var dd = conn.Query<dynamic>("fss_user_menu_dashboard_d_group", Parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public bool UpdateDefaultDashboard(int PortfolioId, int SsGroupID,string DashboardUrl,string UserInput)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_ss_group_id", SsGroupID, dbType: DbType.Int32);
                    Parameters.Add("p_dashboard_url", DashboardUrl);
                    Parameters.Add("p_user_edit", UserInput);
                    var dd = conn.Query<dynamic>("fss_group_u_dashboard_url", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteDetailMenu(int portfolioId, string user_id)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_menu_user WHERE ss_portfolio_id = @ss_portfolio_id AND user_id = @user_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_portfolio_id = portfolioId, user_id = user_id });
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
                        FROM public.vss_user_by  WHERE ss_user_id = @ss_user_id";
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

        public object SelectScalar(SQL.Function.Aggregate function, string column, string ParamWhere)
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
                                      portfolio_id = @portfolio_id,
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
                                      notes = @notes,
                                      is_inactive = @is_inactive
                                    WHERE 
                                      ss_user_id = @ss_user_id
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
