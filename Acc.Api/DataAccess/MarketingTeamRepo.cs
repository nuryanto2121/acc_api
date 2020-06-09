using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class MarketingTeamRepo
    {
        private string connectionString;
        private IConfiguration config;

        public MarketingTeamRepo(IConfiguration Configuration)
        {
            config = Configuration;
            connectionString = Tools.ConnectionString(Configuration);
        }

        public List<MarketingTeam> GetList(string UserId, int ss_portfolio_id)
        {
            List<MarketingTeam> tt = new List<MarketingTeam>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string SqlQuery = @"select a.marketing_id,
                                               a.name,
                                               a.join_date,
                                               a.monthly_point,
                                               a.monthly_new_prospect,
                                               (CASE WHEN b.child_marketing_id is null THEN false else true END) as is_my_team
                                        from mk_marketing a left OUTER join mk_marketing_team b
                                        ON a.marketing_id = b.child_marketing_id
                                        AND a.ss_portfolio_id = b.ss_portfolio_id
                                        AND b.user_id iLIKE @user_id
                                        WHERE a.ss_portfolio_id = @ss_portfolio_id
                                        group by a.marketing_id,
                                               a.name,
                                               a.join_date,
                                               a.monthly_point,
                                               a.monthly_new_prospect,b.child_marketing_id,a.user_id
                                        HAVING a.user_id <> @user_id;";
                    conn.Open();
                    tt = conn.Query<MarketingTeam>(SqlQuery, new { user_id = UserId, ss_portfolio_id = ss_portfolio_id }).ToList();                    
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

        public bool Insert(int p_ss_portfolio_id, string user_id, string ChildMarketing)
        {
            bool _result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    DynamicParameters spParam = new DynamicParameters();
                    string SpName = string.Empty;

                    SpName = "fmk_marketing_team_i";
                    spParam.Add("@p_ss_portfolio_id", p_ss_portfolio_id, dbType: DbType.Int32);
                    spParam.Add("@p_user_id", user_id, dbType: DbType.String);
                    spParam.Add("@p_child_marketing_id", ChildMarketing);
                    spParam.Add("@p_user_input", user_id, dbType: DbType.String);

                    conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);
                    _result = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return _result;
        }
        public bool DeleteDetail(int PortfolioID, string UserID)
        {
            bool _result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    DynamicParameters spParam = new DynamicParameters();
                    string SpName = string.Empty;

                    SpName = "fmk_marketing_team_d";
                    spParam.Add("@p_ss_portfolio_id", PortfolioID, dbType: DbType.Int32);
                    spParam.Add("@p_user_id", UserID, dbType: DbType.String);

                    conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);
                    _result = true;
                }
                catch (Exception ex)
                {
                    throw ex;
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
