using Acc.Api.Helper;
using Dapper;
using Acc.Api.Helper;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Acc.Api.DataAccess
{
    public class AuthRepo
    {
        private string connectionString;
        private FunctionString fn;
        public AuthRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public SsUser GetDataAuthByEmail(string Email)  
        {
            SsUser t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT   ss_user_id,  user_id,
                                          ss_group_id,  user_name,
                                          email,  user_level,
                                          expired_date,  is_inactive,
                                          job_title,  hand_phone,
                                          last_change_password,  default_language,
                                          user_input,  user_edit,
                                          portfolio_id,  subportfolio_id,
                                          time_input,  time_edit,
                                          file_name,  path_file,
                                          address,  notes,  otp
                                    FROM
                                      ss_user WHERE email = @email 
                                    ";
                try
                {
                    conn.Open();
                    t = conn.Query<SsUser>(strQuery, new { email = Email }).SingleOrDefault();
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

        public SsUser GetDataAuthByOTP(string OTP)
        {
            SsUser t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT   ss_user_id,  user_id,
                                          ss_group_id,  user_name,
                                          email,  user_level,
                                          expired_date,  is_inactive,
                                          job_title,  hand_phone,
                                          last_change_password,  default_language,
                                          user_input,  user_edit,
                                          portfolio_id,  subportfolio_id,
                                          time_input,  time_edit,
                                          file_name,  path_file,
                                          address,  notes,  otp
                                    FROM
                                      ss_user WHERE otp = @otp 
                                    ";
                try
                {
                    conn.Open();
                    t = conn.Query<SsUser>(strQuery, new { otp = OTP }).SingleOrDefault();
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

        public SsUser GetDataAuthByUserId(string UserID)
        {
            SsUser t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT   ss_user_id,  user_id,
                                          ss_group_id,  user_name,
                                          email,  user_level,
                                          expired_date,  is_inactive,
                                          job_title,  hand_phone,
                                          last_change_password,  default_language,
                                          user_input,  user_edit,
                                          portfolio_id,  subportfolio_id,
                                          time_input,  time_edit,
                                          file_name,  path_file,
                                          address,  notes,  otp,password
                                    FROM
                                      ss_user WHERE user_id = @UserID 
                                    ";
                try
                {
                    conn.Open();
                    t = conn.Query<SsUser>(strQuery, new { UserID = UserID }).SingleOrDefault();
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

        public bool UpdateOTP(string otp,string email,int ss_user_id)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"UPDATE 
                                      ss_user
                                    SET                                  
                                      otp = @otp
                                    WHERE ss_user_id = @user_id
                                      AND email = @email
                                    ;";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, new { otp = otp, user_id = ss_user_id, email = email });
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
        public DataTable GetDataAuth(AuthLogin Param)
        {
            var dd = new List<dynamic>();
            var data = new DataTable();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var Parameters = new DynamicParameters();
                    string SpName = "_getlogin_auth";
                    Parameters.Add("p_user_id", Param.UserLog, dbType: DbType.String);
                    Parameters.Add("p_password", Param.PassLog, dbType: DbType.String);
                    Parameters.Add("p_subportfolio_short_name", "", dbType: DbType.String);
                    dd = conn.Query<dynamic>(SpName, Parameters, commandType: CommandType.StoredProcedure).ToList();
                    data = fn.ToDataTable(dd);

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

            return data;
        }
        public bool SaveUserSession(UserSession domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO ss_user_session(user_id,token,last_login,expire_on,ip_address,user_input,time_input,user_edit,time_edit) VALUES(@user_id,@token,@last_login,@expire_on,@ip_address,@user_input,@time_input,@user_edit,@time_edit)";
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
        public UserSession GetDataSessionLog(string UserID, string Token)
        {
            UserSession t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT user_session_id,
                                      user_id,token,
                                      last_login,expire_on,
                                      ip_address,user_input,
                                      user_edit,time_input,
                                      time_edit
                                    FROM 
                                      public.ss_user_session 
                                    where user_id = @user_id
                                    And token = @token;";
                try
                {
                    conn.Open();
                    t = conn.Query<UserSession>(strQuery, new { user_id = UserID, token = Token }).SingleOrDefault();
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
        public int CountUserLog(string sParam)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                StringBuilder sqlQuery = new StringBuilder();

                try
                {
                    conn.Open();
                    sParam = !string.IsNullOrEmpty(sParam) ? " AND " + sParam : string.Empty;
                    sqlQuery.AppendFormat("select count(*) as cnt from public.ss_user_log WHERE time_input::date = now()::date {0};", sParam);
                    var res = conn.ExecuteScalar(sqlQuery.ToString());
                    result = Convert.ToInt32(res);


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
        public string MaxCaptchaUserLog(string sParam)
        {
            string result = string.Empty;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                StringBuilder sqlQuery = new StringBuilder();

                try
                {
                    conn.Open();
                    sParam = !string.IsNullOrEmpty(sParam) ? " AND " + sParam : string.Empty;
                    sqlQuery.AppendFormat("select max(captcha) as cnt from public.ss_user_log WHERE time_input::date = now()::date {0};", sParam);
                    var res = conn.ExecuteScalar(sqlQuery.ToString());
                    result = res == null ? result : res.ToString();


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
        public bool SaveUserLog(SsUserLog domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO public.ss_user_log (user_id,ip_address,login_date,logout_date,token,is_fraud,captcha,user_input,time_input,user_edit,time_edit) VALUES (@user_id, @ip_address, @login_date, @logout_date, @token,@is_fraud,@captcha, @user_input,@time_input, @user_edit,@time_edit)";
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

        public bool DeleteUserLog(string sParam)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                sParam = !string.IsNullOrEmpty(sParam) ? " AND " + sParam : string.Empty;
                string sqlQuery = string.Format(@"DELETE from ss_user_log
                                    WHERE time_input::date = now()::date {0} ;", sParam);
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery);
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
        public bool UpdateUserLogisFraud(string sParam)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                sParam = !string.IsNullOrEmpty(sParam) ? " AND " + sParam : string.Empty;
                string sqlQuery = string.Format(@"UPDATE ss_user_log set is_fraud='false', time_edit = now()::timestamp without time zone 
                                    WHERE time_input::date = now()::date {0} ;", sParam);
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery);
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
        public bool DeleteUserSession(UserSession domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"DELETE from ss_user_session
                                    WHERE user_id iLIKE @user_id AND token = @token AND ip_address iLIKE @ip_address; ";
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
        public bool UpdateUserLog(string UserLogin, string Ip, string Token)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"UPDATE 
                                      ss_user_log
                                    SET
                                      logout_date = now()::timestamp,                                     
                                      user_edit = @user_id,
                                      time_edit = now()::timestamp
                                    WHERE user_id = @user_id
                                      AND token = @token
                                      AND ip_address = @ip_address
                                    ;";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, new { user_id = UserLogin, token = Token, ip_address = Ip });
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
        public bool UpdatePass(int ss_user_id, string Pwd)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"UPDATE 
                                      ss_user
                                    SET
                                      password = @Passwrod,
                                      otp=''
                                    WHERE ss_user_id = @ss_user_id
                                    ;";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, new { ss_user_id = ss_user_id, Passwrod = Pwd });
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
    }
}
