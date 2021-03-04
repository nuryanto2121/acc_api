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
    public class InsuranceRepo
    {
        private string connectionString;
        public InsuranceRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public VendorInsurance GetVendorInsurance(string VendorToken)
        {
            VendorInsurance t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT 
                                  cm_vendor_insurance_id,   vendor_code,
                                  vendor_name,              merchant_id,
                                    secret_key,             url,
                                    merchant_id_uat,        secret_key_uat,
                                    url_uat,                is_prod
                                FROM 
                                  public.cm_vendor_insurance 
                                WHERE vendor_code = @vendor_code;";
                try
                {
                    conn.Open();
                    t = conn.Query<VendorInsurance>(strQuery, new { vendor_code = VendorToken }).SingleOrDefault();
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
        public bool IsProdInsurance(int SsPortfolioId)
        {
            bool t = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"select a.is_prod_insurance as row_id
                                    from ss_portfolio a
                                    where a.ss_portfolio_id = @ss_portfolio_id;";
                try
                {
                    conn.Open();
                    var dd = conn.Query<RowID>(strQuery, new { ss_portfolio_id = SsPortfolioId }).SingleOrDefault();
                    t = dd.row_id == 0 ? false : true;
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
        public bool UpdateOpOrderOld(string FileName, string FilePath, string SOT, string PoliceNo)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"    update op_order
                                set insurance_policy_file_name = @insurance_policy_file_name,
                                    insurance_policy_path_file = @insurance_policy_path_file
                                where order_no = @order_no 
                                AND insurance_policy_no = @insurance_policy_no ";
                try
                {
                    conn.Open();
                    result = conn.Execute(sqlQuery, new { order_no = SOT, insurance_policy_no = PoliceNo, insurance_policy_file_name = FileName, insurance_policy_path_file = FilePath });
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
        /*
          conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();                    
                    Parameters.Add("p_order_no", SOT);
                    Parameters.Add("p_insurance_policy_no", PoliceNo);
                    Parameters.Add("p_file_name", FileName);
                    Parameters.Add("p_path_file", FilePath);                    
                    var dd = conn.Query<dynamic>("fss_upload_insurance_order_u", Parameters, commandType: CommandType.StoredProcedure).ToList();
                    _result = true;
         */
        public bool UpdateOpOrder(string FileName, string FilePath, string SOT, string PoliceNo)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_order_no", SOT);
                    Parameters.Add("p_insurance_policy_no", PoliceNo);
                    Parameters.Add("p_file_name", FileName);
                    Parameters.Add("p_path_file", FilePath);
                    var dd = conn.Query<dynamic>("fss_upload_insurance_order_u", Parameters, commandType: CommandType.StoredProcedure).ToList();
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

        public bool InsertLog(VendorInsuranceLog param)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_vendor_token", param.vendor_token);
                    Parameters.Add("p_order_no", param.order_no);
                    Parameters.Add("p_insurance_policy_no", param.insurance_policy_no);
                    Parameters.Add("p_file_name", param.file_name);
                    Parameters.Add("p_path_file", param.path_file);
                    Parameters.Add("p_ip_address", param.ip_address);
                    Parameters.Add("p_user_agent", param.user_agent);
                    Parameters.Add("p_user_input", param.vendor_token);
                    Parameters.Add("p_param_post", param.param_string);                    
                    var dd = conn.Query<dynamic>("fss_upload_insurance_policy_log_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
