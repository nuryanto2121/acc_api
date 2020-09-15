using Acc.Api.Helper;
using Acc.Api.Models.SystemAdministrator;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class SysMenuFavoriteRepo
    {
        private string connectionString;
        private FunctionString fn;
        public SysMenuFavoriteRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public bool Save(SsMenuFavorite domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO public.ss_user_favorite
                                    (
                                      ss_portfolio_id,      user_id,
                                      ss_menu_id,           user_input,
                                      user_edit,            time_input,
                                      time_edit
                                    )
                                    VALUES (
                                      @ss_portfolio_id,     @user_id,
                                      @ss_menu_id,          @user_input,
                                      @user_edit,           @time_input,
                                      @time_edit
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
        public bool Update(SsMenuFavorite domain)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                string sqlQuery = @"UPDATE 
                      public.ss_user_favorite 
                    SET 
                      ss_portfolio_id = @ss_portfolio_id,
                      user_id = @user_id,
                      ss_menu_id = @ss_menu_id,
                      user_edit = @user_edit,
                      time_edit = @time_edit
                    WHERE 
                      ss_user_favorite_id = @ss_user_favorite_id
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
        public bool Delete(SsMenuFavorite domain)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"DELETE FROM public.ss_user_favorite 
                                    WHERE ss_user_favorite_id = @ss_user_favorite_id
                                    AND user_id = @user_id
                                    AND ss_portfolio_id = @ss_portfolio_id;";
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

        public object getMenuFavorite(int PortfolioId,string UserId)
        {
            object _result = new object();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {

                    string sQuery = @"select 	f.ss_user_favorite_id,sm.ss_menu_id,sm.title, sm.menu_url,
		                                    sm.menu_type,sm.parent_menu_id,sm.icon_class ,
                                            sm.order_seq, sm.level_no,smg.add_status,smg.edit_status,smg.delete_status 
                                     from ss_user_favorite f inner join ss_user su
                                            ON f.user_id = su.user_id
                                            AND f.ss_portfolio_id = su.portfolio_id
 	                                    inner join ss_menu sm
    	                                    ON sm.ss_menu_id = f.ss_menu_id
	                                    left outer join ss_menu_group smg
    	                                    ON smg.ss_portfolio_id = f.ss_portfolio_id
                                            AND smg.ss_group_id = su.ss_group_id
                                            and smg.ss_menu_id = sm.ss_menu_id          
                                       where f.ss_portfolio_id = @ss_portfolio_id
                                        and f.user_id iLIKE @user_id      
                                        order by case when sm.title ILIKE'%dashboard%' then 0 else 1 end,f.row_no ;";
                    conn.Open();
                    _result = conn.Query<dynamic>(sQuery,new { ss_portfolio_id = PortfolioId, user_id = UserId }).ToList();
                    
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
        public SsMenuFavorite GetById(int key)
        {
            SsMenuFavorite t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT 
                                  ss_user_favorite_id,
                                  ss_portfolio_id,
                                  user_id,
                                  ss_menu_id,
                                  user_input,
                                  user_edit,
                                  time_input,
                                  time_edit
                                FROM 
                                  public.ss_user_favorite 
                        WHERE ss_user_favorite_id = @ss_user_favorite_id;";
                try
                {
                    conn.Open();
                    t = conn.Query<SsMenuFavorite>(strQuery, new { ss_user_favorite_id = key }).SingleOrDefault();
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
