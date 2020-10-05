using Acc.Api.Helper;
using Dapper;
using Newtonsoft.Json.Linq;
using Acc.Api.Enum;
//using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class SsMenuRepo : IRepository<SsMenu, int>
    {
        private string connectionString;
        private FunctionString fn;
        public SsMenuRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public object SelectScalar(SQL.Function.Aggregate function, string column, string ParamWhere)
        {
            object _result = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                StringBuilder sbQuery = new StringBuilder();
                switch (function)
                {
                    case SQL.Function.Aggregate.Max:
                        sbQuery.AppendFormat("SELECT MAX({0}) FROM public.ss_menu ", column);
                        break;
                    case SQL.Function.Aggregate.Min:
                        sbQuery.AppendFormat("SELECT MIN({0}) FROM public.ss_menu ", column);
                        break;
                    case SQL.Function.Aggregate.Distinct:
                        sbQuery.AppendFormat("SELECT DISTINCT({0}) FROM public.ss_menu ", column);
                        break;
                    case SQL.Function.Aggregate.Count:
                        sbQuery.AppendFormat("SELECT COUNT({0}) FROM public.ss_menu ", column);
                        break;
                    case SQL.Function.Aggregate.Sum:
                        sbQuery.AppendFormat("SELECT SUM({0}) FROM public.ss_menu ", column);
                        break;
                    case SQL.Function.Aggregate.Avg:
                        sbQuery.AppendFormat("SELECT AVG({0}) FROM public.ss_menu ", column);
                        break;
                    default:
                        // do nothing 
                        break;
                }

                try
                {
                    conn.Open();
                    _result = conn.ExecuteScalar(sbQuery.ToString());
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
        public List<SsMenu> GetList()
        {
            List<SsMenu> tt = new List<SsMenu>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string sQuery = @"SELECT 
                                      ss_menu_id,title,menu_url,menu_type,parent_menu_id,icon_class,order_seq,ss_module_id,user_input,user_edit,time_input,time_edit,level_no
                                    FROM 
                                      public.ss_menu ;";
                    conn.Open();
                    tt = conn.Query<SsMenu>(sQuery).ToList();
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
            return tt;
        }
        public List<SsMenu> GetList(string Parameter)
        {
            List<SsMenu> tt = new List<SsMenu>();
            //Parameter = !string.IsNullOrEmpty(Parameter) ? "WHERE " + Parameter : Parameter;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string sQuery = string.Format(@"SELECT 
                                        ss_menu_id,  title,  menu_url,  menu_type,  parent_menu_id,  icon_class,  order_seq,  ss_module_id,  user_input,  user_edit,  time_input,  time_edit,  level_no
                                    FROM 
                                      public.ss_menu {0} order by parent_menu_id asc,level_no asc,order_seq asc;", Parameter);
                    conn.Open();
                    tt = conn.Query<SsMenu>(sQuery).ToList();
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
            return tt;
        }
        public List<SsMenu> GetList(int start, int pageSize, string sortName, string sortOrder, string Parameter)
        {
            List<SsMenu> tt = new List<SsMenu>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                int startRow = (start + 1);
                int endRow = (start + pageSize);

                StringBuilder sbQuery = new StringBuilder();
                sbQuery.AppendFormat(" WITH result_set AS ");
                sbQuery.AppendFormat(" ( ");
                sbQuery.AppendFormat("    SELECT ");
                sbQuery.AppendFormat("      ROW_NUMBER() OVER (ORDER BY {0} {1}) AS [row_number], ", sortName, sortOrder);
                sbQuery.AppendFormat("          ss_menu_id,  title,  menu_url,  menu_type,  parent_menu_id,  icon_class,  order_seq,  ss_module_id,  user_input,  user_edit,  time_input,  time_edit,  level_no ");
                sbQuery.AppendFormat("    FROM ");
                sbQuery.AppendFormat("      public.ss_menu  ");
                sbQuery.AppendFormat(" {0} ", Parameter);
                sbQuery.AppendFormat(" ) ");
                sbQuery.AppendFormat(" SELECT * FROM result_set WHERE [row_number] BETWEEN {0} AND {1} ", startRow, endRow);

                try
                {
                    conn.Open();
                    tt = conn.Query<SsMenu>(sbQuery.ToString()).ToList<SsMenu>();
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

            return tt;
        }
        public SsMenu GetById(int key)
        {
            SsMenu t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = "SELECT   ss_menu_id,  title,  menu_url,  menu_type,  parent_menu_id,  icon_class,  order_seq,  ss_module_id,  user_input,  user_edit,  time_input,  time_edit,  level_no FROM public.ss_menu  WHERE ss_menu_id = @ss_menu_id";
                try
                {
                    conn.Open();
                    t = conn.Query<SsMenu>(strQuery, new { ss_menu_id = key }).SingleOrDefault();
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
        public bool Save(SsMenu domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO public.ss_menu (
                                      title,                        menu_url,
                                      menu_type,                    parent_menu_id,
                                      icon_class,                   order_seq,
                                      ss_module_id,                 user_input,
                                      user_edit,                    time_input,
                                      time_edit,                    level_no
                                )
                                VALUES (
                                      @title,                        @menu_url,
                                      @menu_type,                    @parent_menu_id,
                                      @icon_class,                   @order_seq,
                                      @ss_module_id,                 @user_input,
                                      @user_edit,                    @time_input,
                                      @time_edit,                    @level_no
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
        public bool Update(SsMenu domain)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                //string sqlQuery = "UPDATE public.ss_menu SET title = @title,url = @menu_url,parent_menu_id = @parent_menu_id,parent_menu_title = @icon_class = @icon_class,path = @path,order_seq = @order_seq,user_edit = @user_edit,time_edit = @time_edit WHERE ss_menu_id = @ss_menu_id";
                //AND xmin::text::integer = @lastupdatestamp";
                string sqlQuery = @"UPDATE public.ss_menu 
                                    SET 
                                      title = @title,
                                      menu_url = @menu_url,
                                      menu_type = @menu_type,
                                      parent_menu_id = @parent_menu_id,
                                      icon_class = @icon_class,
                                      order_seq = @order_seq,
                                      ss_module_id = @ss_module_id,
                                      user_edit = @user_edit,
                                      time_edit = @time_edit
                                    WHERE 
                                      ss_menu_id = @ss_menu_id
                                    ;";

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
        public bool Delete(int key)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_menu WHERE ss_menu_id = @ss_menu_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_menu_id = key });
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
        public DataTable getMenuGroup(int PortfolioId, int GroupId,string UserID)
        {
            DataTable _result = new DataTable();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_group_id", GroupId, dbType: DbType.Int32);
                    Parameters.Add("p_user_id", UserID);
                    var dd = conn.Query<dynamic>("fss_menu_list_s", Parameters, commandType: CommandType.StoredProcedure).ToList();
                    _result = fn.ToDataTable(dd);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public object GetMenuJson(int? portfolio_id, int? group_id)
        {
            object _result = new object();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    if (group_id == 0)
                    {
                        Parameters.Add("p_ss_group_id", null, dbType: DbType.Int32);
                    }
                    else
                    {
                        Parameters.Add("p_ss_group_id", group_id, dbType: DbType.Int32);
                    }
                    if (portfolio_id == 0)
                    {
                        Parameters.Add("p_ss_portfolio_id", null, dbType: DbType.Int32);
                    }
                    else
                    {
                        Parameters.Add("p_ss_portfolio_id", portfolio_id, dbType: DbType.Int32);
                    }
                    
                    
                    var dd = conn.Query<dynamic>("get_menu_json_group", Parameters, commandType: CommandType.StoredProcedure);
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
        public bool SaveTemp(SsMenuTemp domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO public.ss_menu_temp (
                                      ss_menu_id,
                                      title,                        menu_url,
                                      menu_type,                    parent_menu_id,
                                      icon_class,                   order_seq,
                                      ss_module_id,                 level_no,
                                      on_event,
                                      time_input,                   user_input
                                )
                                VALUES (
                                      @ss_menu_id,
                                      @title,                        @menu_url,
                                      @menu_type,                    @parent_menu_id,
                                      @icon_class,                   @order_seq,
                                      @ss_module_id,                 @level_no,
                                      @on_event,
                                      @time_input,                   @user_input
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
        public bool SaveTempBydIdMenu(int id, string on_event)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO public.ss_menu_temp (
                                      ss_menu_id,
                                      title,                        menu_url,
                                      menu_type,                    parent_menu_id,
                                      icon_class,                   order_seq,
                                      ss_module_id,                 level_no,
                                      on_event,
                                      time_input,                   user_input
                                )
                                select ss_menu_id,
                                      title,                        menu_url,
                                      menu_type,                    parent_menu_id,
                                      icon_class,                   order_seq,
                                      ss_module_id,                 level_no,
                                      @on_event,
                                      time_input,                   user_input
                               from ss_menu
                            WHERE ss_menu_id = @id;";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, new { id = id, on_event = on_event });
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

        public bool Delete(int key, int timestamp)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_menu WHERE ss_menu_id = @ss_menu_id and xmin::text::integer = @timestamp";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_menu_id = key, timestamp = timestamp });
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

        public SsMenu GetById(int key, int timestamp)
        {
            SsMenu t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = "SELECT ss_menu_id,  title,  menu_url,  menu_type,  parent_menu_id,  icon_class,  order_seq,  ss_module_id,  user_input,  user_edit,  time_input,  time_edit,  level_no FROM public.ss_menu  WHERE ss_menu_id = @ss_menu_id";
                try
                {
                    conn.Open();
                    t = conn.Query<SsMenu>(strQuery, new { ss_menu_id = key }).SingleOrDefault();
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
    }
}
