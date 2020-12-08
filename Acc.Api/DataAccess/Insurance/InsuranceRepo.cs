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
        public VendorInsurance GetVendor(string VendorToken)
        {
            VendorInsurance t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT 
                                  cm_vendor_insurance_id,
                                  vendor_code,
                                  vendor_name
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
        public bool UpdateOpOrder(string FileName, string FilePath, string SOT, string PoliceNo)
        {
            int result = 0;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"    update op_order
                                set insurance_policy_file_name = @insurance_policy_file_name,
                                    insurance_policy_path_file = @insurance_policy_path_file
                                where order_no = @order_no AND insurance_policy_no = @insurance_policy_no ";
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
    }
}
