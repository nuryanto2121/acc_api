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
    public class TrackingRepo
    {
        private string connectionString;
        private FunctionString Fn;
        public TrackingRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            Fn = new FunctionString(connectionString);
        }

        public object GetDataTracking(int ID)
        {
            object _result = new object();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("p_row_id", ID, dbType: DbType.Int32);

                    Parameters.Add("p_lastupdatestamp", 0, dbType: DbType.Int32);

                    var dd = conn.Query<dynamic>("fop_order_tracking_s", Parameters, commandType: CommandType.StoredProcedure);
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
        public RowID GetRowID(string OrderNo)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_order_no", OrderNo);
                    _result = conn.Query<RowID>("get_tracking_op_order_id", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        
        public RowID SaveLog(string Captcha,string AccessWith)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ip_address", Tools.GetIpAddress());
                    Parameters.Add("p_access_with", AccessWith);
                    Parameters.Add("p_order_no", "");
                    Parameters.Add("p_captcha", Captcha);
                    Parameters.Add("p_user_input", "");
                    _result = conn.Query<RowID>("fss_tracking_access_log_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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

        public TrackingAccessLog GetDataLog(string IpAddress, string Captcha,string AccessWith)
        {
            TrackingAccessLog t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"select ss_tracking_access_log_id,ip_address,access_with,captcha,order_no 
                                    from ss_tracking_access_log 
                                    where ip_address = @ip_address 
                                    and captcha = @captcha
                                    and access_with = @access_with;";
                try
                {
                    conn.Open();
                    t = conn.Query<TrackingAccessLog>(strQuery, new { ip_address = IpAddress, captcha = Captcha, access_with = AccessWith }).SingleOrDefault();
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

        public bool UpdateChaptcha(string IpAddress, string Captcha, string AccessWith)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @" update ss_tracking_access_log
                                set captcha = ''
                                where ip_address = @ip_address 
                                and captcha = @captcha
                                and access_with = @access_with;";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ip_address = IpAddress, captcha = Captcha, access_with = AccessWith });
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

        public bool DeleteChaptcha(string IpAddress, string AccessWith)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @" DELETE FROM ss_tracking_access_log
                                where ip_address = @ip_address 
                                and captcha <> ''
                                and access_with = @access_with;";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { ip_address = IpAddress, access_with = AccessWith });
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
