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
    public class UserMgmRepo
    {
        private string connectionString;
        private FunctionString fn;
        public UserMgmRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }

        public RowID Save(UserManagement Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_user_name", Model.user_name);
                    Parameters.Add("p_password", Model.password);
                    Parameters.Add("p_email", Model.email);
                    Parameters.Add("p_expired_date", Model.expired_date, dbType: DbType.DateTime);
                    Parameters.Add("p_is_inactive", Model.is_inactive);
                    Parameters.Add("p_job_title", Model.job_title);
                    Parameters.Add("p_hand_phone", Model.hand_phone);
                    Parameters.Add("p_last_change_password", Model.last_change_password, dbType: DbType.DateTime);
                    Parameters.Add("p_default_language", Model.default_language);
                    Parameters.Add("p_user_input", Model.user_input);
                    Parameters.Add("p_portfolio_id", Convert.ToInt32(Model.portfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_subportfolio_id", Convert.ToInt32(Model.subportfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_file_name", Model.file_name);
                    Parameters.Add("p_path_file", Model.path_file);
                    Parameters.Add("p_address", Model.address);                                                                                                                        
                    Parameters.Add("p_fcm_token_android", Model.fcm_token_android);
                    Parameters.Add("p_fcm_token_ios", Model.fcm_token_ios);
                    
                    _result = conn.Query<RowID>("fss_user_management_mobile_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public bool Update(UserManagement Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("p_ss_user_management_mobile_id", Convert.ToInt32(Model.ss_user_management_mobile_id), dbType: DbType.Int32);
                    Parameters.Add("p_portfolio_id", Convert.ToInt32(Model.portfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_subportfolio_id", Convert.ToInt32(Model.subportfolio_id), dbType: DbType.Int32);
                    Parameters.Add("p_user_name", Model.user_name);
                    Parameters.Add("p_hand_phone", Model.hand_phone);
                    Parameters.Add("p_password", Model.password);
                    Parameters.Add("p_email", Model.email);
                    Parameters.Add("p_expired_date", Model.expired_date, dbType: DbType.DateTime);
                    Parameters.Add("p_is_inactive", Model.is_inactive);
                    Parameters.Add("p_job_title", Model.job_title);
                    Parameters.Add("p_last_change_password", Model.last_change_password, dbType: DbType.DateTime);
                    Parameters.Add("p_lastupdatestamp", Model.lastupdatestamp);                    
                    Parameters.Add("p_default_language", Model.default_language);
                    Parameters.Add("p_file_name", Model.file_name);
                    Parameters.Add("p_path_file", Model.path_file);
                    Parameters.Add("p_address", Model.address);
                    Parameters.Add("p_fcm_token_android", Model.fcm_token_android);
                    Parameters.Add("p_fcm_token_ios", Model.fcm_token_ios);
                    Parameters.Add("p_user_edit", Model.user_edit);
                    var dd = conn.Query<RowID>("fss_user_management_mobile_u", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
