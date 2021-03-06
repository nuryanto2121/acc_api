﻿using Acc.Api.Enum;
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
    public class SsGroupRepo : IRepository<SsGroup, int>
    {
        private string connectionString;
        private FunctionString fn;
        public SsGroupRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public object GetMenuJson(int? portfolio_id, int? group_id, string group_access)
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
                    Parameters.Add("p_group_access", group_access);

                    var dd = conn.Query<dynamic>("get_menu_json_group", Parameters, commandType: CommandType.StoredProcedure);
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
        public bool Delete(int key, int timestamp)
        {
            throw new NotImplementedException();
        }
        public bool DeleteButtonGroup(int PortfolioId, int GroupID)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_ss_group_id", GroupID, dbType: DbType.Int32);
                    var dd = conn.Query<dynamic>("fss_group_menu_button_access_d", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteDashboardGroup(int PortfolioId, int GroupID)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", PortfolioId, dbType: DbType.Int32);
                    Parameters.Add("p_ss_group_id", GroupID, dbType: DbType.Int32);
                    var dd = conn.Query<dynamic>("fss_group_menu_dashboard_d_group", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteDetail(int portfolioId, int group_id)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "DELETE FROM public.ss_menu_group WHERE ss_portfolio_id = @ss_portfolio_id AND ss_group_id = @ss_group_id";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ss_portfolio_id = portfolioId, ss_group_id = group_id });
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

        public SsGroup GetById(int key, int timestamp)
        {
            throw new NotImplementedException();
        }

        public List<SsGroup> GetList()
        {
            throw new NotImplementedException();
        }

        public List<SsGroup> GetList(int pageSize, int currentPage, string sortName, string sortOrder, string Parameter)
        {
            throw new NotImplementedException();
        }

        public RowID SaveHeader(SsGroup Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    Model.group_access = string.IsNullOrEmpty(Model.group_access) ? Model.user_type : Model.group_access;
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Model.ss_portfolio_id, dbType: DbType.Int32);
                    Parameters.Add("p_descs", Model.descs);
                    Parameters.Add("p_short_descs", Model.short_descs);
                    Parameters.Add("p_user_type", Model.user_type);
                    Parameters.Add("p_dashboard_url", Model.dashboard_url);
                    Parameters.Add("p_group_access", Model.group_access);
                    Parameters.Add("p_user_input", Model.user_input);
                    _result = conn.Query<RowID>("fss_group_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public bool SaveDetail(SsMenuGroup Model)
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
                    Parameters.Add("p_ss_group_id", Model.ss_group_id);
                    Parameters.Add("p_add_status", Model.add_status, dbType: DbType.Boolean);
                    Parameters.Add("p_edit_status", Model.edit_status, dbType: DbType.Boolean);
                    Parameters.Add("p_delete_status", Model.delete_status, dbType: DbType.Boolean);
                    Parameters.Add("p_view_status", Model.view_status, dbType: DbType.Boolean);
                    Parameters.Add("p_post_status", Model.post_status, dbType: DbType.Boolean);
                    Parameters.Add("p_user_input", Model.user_input);
                    var dd = conn.Query<dynamic>("fss_menu_group_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public object SelectScalar(SQL.Function.Aggregate function, string column, string ParamWhere)
        {
            object _result = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                ParamWhere = string.IsNullOrEmpty(ParamWhere) ? string.Empty : "WHERE " + ParamWhere;
                StringBuilder sbQuery = new StringBuilder();
                switch (function)
                {
                    case SQL.Function.Aggregate.Max:
                        sbQuery.AppendFormat("SELECT MAX({0}) FROM public.ss_group {1}", column, ParamWhere);
                        break;
                    case SQL.Function.Aggregate.Min:
                        sbQuery.AppendFormat("SELECT MIN({0}) FROM public.ss_group {1}", column, ParamWhere);
                        break;
                    case SQL.Function.Aggregate.Distinct:
                        sbQuery.AppendFormat("SELECT DISTINCT({0}) FROM public.ss_group {1}", column, ParamWhere);
                        break;
                    case SQL.Function.Aggregate.Count:
                        sbQuery.AppendFormat("SELECT COUNT({0}) FROM public.ss_group {1}", column, ParamWhere);
                        break;
                    case SQL.Function.Aggregate.Sum:
                        sbQuery.AppendFormat("SELECT SUM({0}) FROM public.ss_group {1}", column, ParamWhere);
                        break;
                    case SQL.Function.Aggregate.Avg:
                        sbQuery.AppendFormat("SELECT AVG({0}) FROM public.ss_group {1}", column, ParamWhere);
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

        public bool Update(SsGroup domain)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_group_id", domain.ss_group_id, dbType: DbType.Int32);
                    Parameters.Add("p_ss_portfolio_id", domain.ss_portfolio_id, dbType: DbType.Int32);
                    Parameters.Add("p_descs", domain.descs, dbType: DbType.String);
                    Parameters.Add("p_short_descs", domain.short_descs, dbType: DbType.String);
                    Parameters.Add("p_user_type", domain.user_type, dbType: DbType.String);
                    Parameters.Add("p_dashboard_url", domain.dashboard_url);
                    Parameters.Add("p_group_access", domain.group_access);
                    Parameters.Add("p_lastupdatestamp", domain.lastupdatestamp, dbType: DbType.Int32);
                    Parameters.Add("p_user_edit", domain.user_edit);
                    var dd = conn.Query<dynamic>("fss_group_u", Parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public bool Save(SsGroup domain)
        {
            throw new NotImplementedException();
        }
    }
}
