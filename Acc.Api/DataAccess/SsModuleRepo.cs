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
    public class SsModuleRepo : IRepository<SsModule, int>
    {
        private string connectionString;
        private FunctionString fn;
        public SsModuleRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public object SelectScalar(SQL.Function.Aggregate function, string column)
        {
            object _result = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                StringBuilder sbQuery = new StringBuilder();
                switch (function)
                {
                    case SQL.Function.Aggregate.Max:
                        sbQuery.AppendFormat("SELECT MAX({0}) FROM public.ss_module ", column);
                        break;
                    case SQL.Function.Aggregate.Min:
                        sbQuery.AppendFormat("SELECT MIN({0}) FROM public.ss_module ", column);
                        break;
                    case SQL.Function.Aggregate.Distinct:
                        sbQuery.AppendFormat("SELECT DISTINCT({0}) FROM public.ss_module ", column);
                        break;
                    case SQL.Function.Aggregate.Count:
                        sbQuery.AppendFormat("SELECT COUNT({0}) FROM public.ss_module ", column);
                        break;
                    case SQL.Function.Aggregate.Sum:
                        sbQuery.AppendFormat("SELECT SUM({0}) FROM public.ss_module ", column);
                        break;
                    case SQL.Function.Aggregate.Avg:
                        sbQuery.AppendFormat("SELECT AVG({0}) FROM public.ss_module ", column);
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
        public List<SsModule> GetList()
        {
            List<SsModule> tt = new List<SsModule>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string sQuery = @"SELECT 
                                        ss_module_id,  descs,  short_descs,  user_input,  user_edit,  time_input,  time_edit
                                    FROM 
                                      public.ss_module ;";
                    conn.Open();
                    tt = conn.Query<SsModule>(sQuery).ToList();
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
        public List<SsModule> GetList(string Parameter)
        {
            List<SsModule> tt = new List<SsModule>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string sQuery = string.Format(@"SELECT 
                                        ss_module_id,  descs,  short_descs,  user_input,  user_edit,  time_input,  time_edit
                                    FROM 
                                      public.ss_module {0} ;", Parameter);
                    conn.Open();
                    tt = conn.Query<SsModule>(sQuery).ToList();
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
        public List<SsModule> GetList(int start, int pageSize, string sortName, string sortOrder, string Parameter)
        {
            List<SsModule> tt = new List<SsModule>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                int startRow = (start + 1);
                int endRow = (start + pageSize);

                StringBuilder sbQuery = new StringBuilder();
                sbQuery.AppendFormat(" WITH result_set AS ");
                sbQuery.AppendFormat(" ( ");
                sbQuery.AppendFormat("    SELECT ");
                sbQuery.AppendFormat("      ROW_NUMBER() OVER (ORDER BY {0} {1}) AS [row_number], ", sortName, sortOrder);
                sbQuery.AppendFormat("          ss_module_id,  descs,  short_descs,  user_input,  user_edit,  time_input,  time_edit ");
                sbQuery.AppendFormat("    FROM ");
                sbQuery.AppendFormat("      public.ss_module  ");
                sbQuery.AppendFormat(" {0} ", Parameter);
                sbQuery.AppendFormat(" ) ");
                sbQuery.AppendFormat(" SELECT * FROM result_set WHERE [row_number] BETWEEN {0} AND {1} ", startRow, endRow);

                try
                {
                    conn.Open();
                    tt = conn.Query<SsModule>(sbQuery.ToString()).ToList<SsModule>();
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
        public SsModule GetById(int key)
        {
            SsModule t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = "SELECT ss_module_id,  title,  module_url,  parent_module_id,    icon_class,  path,  order_seq,  user_input,  user_edit,  time_input,  time_edit FROM public.ss_module  WHERE ss_module_id = @ss_module_id";
                try
                {
                    conn.Open();
                    t = conn.Query<SsModule>(strQuery, new { ss_module_id = key }).SingleOrDefault();
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
        public bool Save(SsModule domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO 
                                      public.ss_module
                                    (
                                      descs,  short_descs,  
                                      user_input,  user_edit,  
                                      time_input,  time_edit
                                    )
                                    VALUES (
                                      @descs,  @short_descs,
                                      @user_input,  @user_edit,
                                      @time_input,  @time_edit
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
        public bool Update(SsModule domain)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                //string sqlQuery = "UPDATE public.ss_module SET title = @title,url = @module_url,parent_module_id = @parent_module_id,parent_module_title = @icon_class = @icon_class,path = @path,order_seq = @order_seq,user_edit = @user_edit,time_edit = @time_edit WHERE ss_module_id = @ss_module_id";
                //AND xmin::text::integer = @lastupdatestamp";
                string sqlQuery = @"UPDATE 
                                  public.ss_module 
                                SET 
                                  descs = @descs,
                                  short_descs = @short_descs,
                                  user_edit = @user_edit,
                                  time_edit = @time_edit
                                WHERE 
                                  ss_module_id = @ss_module_id
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
                string sqlQuery = "DELETE FROM public.ss_module WHERE ss_module_id = @ss_module_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_module_id = key });
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
