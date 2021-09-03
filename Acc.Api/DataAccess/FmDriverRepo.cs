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
    public class FmDriverRepo
    {
        private string connectionString;
        private FunctionString fn;
        public FmDriverRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public RowID SaveHeader(VmFMDriver Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Convert.ToInt32(Model.SsPortfolioId), dbType: DbType.Int32);
                    Parameters.Add("p_employee_id", Model.FMDriver.EmployeeId);
                    Parameters.Add("p_driver_name", Model.FMDriver.DriverName);
                    Parameters.Add("p_handphone", Model.FMDriver.Handphone);
                    Parameters.Add("p_ktp", Model.FMDriver.Ktp);
                    Parameters.Add("p_npwp", Model.FMDriver.Npwp);
                    Parameters.Add("p_sim", Model.FMDriver.Sim);
                    Parameters.Add("p_sim_expiry_date", Model.FMDriver.SimExpiryDate, dbType: DbType.DateTime);
                    Parameters.Add("p_fm_sim_type_id", Model.FMDriver.FmSimTypeId, dbType: DbType.Int32);
                    Parameters.Add("p_skck", Model.FMDriver.Skck);
                    Parameters.Add("p_skck_expiry_date", Model.FMDriver.SkckExpiryDate, dbType: DbType.DateTime);
                    Parameters.Add("p_employee_status", Model.FMDriver.EmployeeStatus);
                    Parameters.Add("p_employee_expiry_date", Model.FMDriver.EmployeeExpiryDate, dbType: DbType.DateTime);
                    Parameters.Add("p_emergency_contact_name", Model.FMDriver.EmergencyContactName);
                    Parameters.Add("p_emergency_relation", Model.FMDriver.EmergencyRelation);
                    Parameters.Add("p_emergency_phone_no", Model.FMDriver.EmergencyPhoneNo);
                    Parameters.Add("p_emergency_remarks", Model.FMDriver.EmergencyRemarks);
                    Parameters.Add("p_address", Model.FMDriver.Address);
                    Parameters.Add("p_bank_name", Model.FMDriver.BankName);
                    Parameters.Add("p_bank_acct_no", Model.FMDriver.BankAcctNo);
                    Parameters.Add("p_user_input", Model.UserInput);
                    Parameters.Add("p_file_name", Model.FMDriver.FileName);
                    Parameters.Add("p_path_file", Model.FMDriver.PathFile);
                    Parameters.Add("p_join_date", Model.FMDriver.JoinDate, dbType: DbType.DateTime);
                    Parameters.Add("p_contract_end_date", Model.FMDriver.ContractEndDate, dbType: DbType.DateTime);
                    Parameters.Add("p_terminate_date", Model.FMDriver.TerminateDate, dbType: DbType.DateTime);
                    Parameters.Add("p_password", Model.FMDriver.Password);
                    Parameters.Add("p_date_of_birth", Model.FMDriver.DateOfBirth, dbType: DbType.DateTime);
                    Parameters.Add("p_expected_income_month_amt", Convert.ToDecimal(Model.FMDriver.ExpectedIncomeMonthAmt), dbType: DbType.Decimal);
                    Parameters.Add("p_expected_order_month", Convert.ToDecimal(Model.FMDriver.ExpectedOrderMonth), dbType: DbType.Decimal);
                    Parameters.Add("p_expected_mileage_month_amt", Convert.ToDecimal(Model.FMDriver.ExpectedMileageMonthAmt), dbType: DbType.Decimal);
                    _result = conn.Query<RowID>("ffm_driver_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public bool UpdateHeader(VmFMDriver Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_fm_driver_id", Model.FMDriver.FMDriverId, dbType: DbType.Int32);
                    Parameters.Add("p_ss_portfolio_id", Convert.ToInt32(Model.SsPortfolioId), dbType: DbType.Int32);
                    Parameters.Add("p_employee_id", Model.FMDriver.EmployeeId);
                    Parameters.Add("p_driver_name", Model.FMDriver.DriverName);
                    Parameters.Add("p_handphone", Model.FMDriver.Handphone);
                    Parameters.Add("p_ktp", Model.FMDriver.Ktp);
                    Parameters.Add("p_npwp", Model.FMDriver.Npwp);
                    Parameters.Add("p_sim", Model.FMDriver.Sim);
                    Parameters.Add("p_sim_expiry_date", Model.FMDriver.SimExpiryDate);
                    Parameters.Add("p_fm_sim_type_id", Model.FMDriver.FmSimTypeId);
                    Parameters.Add("p_skck", Model.FMDriver.Skck);
                    Parameters.Add("p_skck_expiry_date", Model.FMDriver.SkckExpiryDate);
                    Parameters.Add("p_employee_status", Model.FMDriver.EmployeeStatus);
                    Parameters.Add("p_employee_expiry_date", Model.FMDriver.EmployeeExpiryDate);
                    Parameters.Add("p_emergency_contact_name", Model.FMDriver.EmergencyContactName);
                    Parameters.Add("p_emergency_relation", Model.FMDriver.EmergencyRelation);
                    Parameters.Add("p_emergency_phone_no", Model.FMDriver.EmergencyPhoneNo);
                    Parameters.Add("p_emergency_remarks", Model.FMDriver.EmergencyRemarks);
                    Parameters.Add("p_address", Model.FMDriver.Address);
                    Parameters.Add("p_bank_name", Model.FMDriver.BankName);
                    Parameters.Add("p_bank_acct_no", Model.FMDriver.BankAcctNo);
                    Parameters.Add("p_file_name", Model.FMDriver.FileName);
                    Parameters.Add("p_path_file", Model.FMDriver.PathFile);
                    Parameters.Add("p_join_date", Model.FMDriver.JoinDate);
                    Parameters.Add("p_contract_end_date", Model.FMDriver.ContractEndDate);
                    Parameters.Add("p_terminate_date", Model.FMDriver.TerminateDate);
                    Parameters.Add("p_password", Model.FMDriver.Password);
                    Parameters.Add("p_user_edit", Model.UserInput);
                    Parameters.Add("p_lastupdatestamp", 0);
                    Parameters.Add("p_date_of_birth", Model.FMDriver.DateOfBirth, dbType: DbType.DateTime);
                    Parameters.Add("p_expected_income_month_amt", Convert.ToDecimal(Model.FMDriver.ExpectedIncomeMonthAmt), dbType: DbType.Decimal);
                    Parameters.Add("p_expected_order_month", Convert.ToDecimal(Model.FMDriver.ExpectedOrderMonth), dbType: DbType.Decimal);
                    Parameters.Add("p_expected_mileage_month_amt", Convert.ToDecimal(Model.FMDriver.ExpectedMileageMonthAmt), dbType: DbType.Decimal);
                    var dd = conn.Query<dynamic>("ffm_driver_u", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool SaveDocument(DriverDocument Model, int RowID, string UserInput)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_fm_driver_id", RowID, dbType: DbType.Int32);
                    Parameters.Add("p_doc_type", Model.DocType);
                    Parameters.Add("p_doc_no", Model.DocNo);
                    Parameters.Add("p_doc_file_name", Model.DocFileName);
                    Parameters.Add("p_doc_path_file", Model.DocPathFile);
                    Parameters.Add("p_expiry_date", Model.ExpiryDate, dbType: DbType.DateTime);
                    Parameters.Add("p_user_input", UserInput);
                    var dd = conn.Query<dynamic>("ffm_driver_doc_i", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public bool DeleteDocument(int RowID)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_row_id", RowID, dbType: DbType.Int32);
                    Parameters.Add("p_lastupdatestamp", RowID, dbType: DbType.Int32);
                    var dd = conn.Query<dynamic>("ffm_driver_doc_d", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
        public RowID SaveWoUser(MMWorkshopUser Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Convert.ToInt32(Model.SsPortfolioId), dbType: DbType.Int32);
                    Parameters.Add("p_mm_workshop_id", Model.MmWorkshopId, dbType: DbType.Int32);
                    Parameters.Add("p_user_name", Model.UserName);
                    Parameters.Add("p_password", Model.Password);
                    Parameters.Add("p_phone_no", Model.PhoneNo);
                    Parameters.Add("p_descs", Model.Descs);
                    Parameters.Add("p_user_status", Model.UserStatus);
                    Parameters.Add("p_file_name", Model.FileName);
                    Parameters.Add("p_path_file", Model.PathFile);
                    Parameters.Add("p_user_input", Model.UserStatus);                   
                    _result = conn.Query<RowID>("fmm_workshop_user_i", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public bool UpdateWoUser(MMWorkshopUser Model)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                var _result = false;
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_ss_portfolio_id", Convert.ToInt32(Model.SsPortfolioId), dbType: DbType.Int32);
                    Parameters.Add("p_mm_workshop_id", Model.MmWorkshopId, dbType: DbType.Int32);
                    Parameters.Add("p_mm_workshop_user_id", Model.MmWorkshopUserId, dbType: DbType.Int32);
                    Parameters.Add("p_user_name", Model.UserName);
                    Parameters.Add("p_password", Model.Password);
                    Parameters.Add("p_phone_no", Model.PhoneNo);
                    Parameters.Add("p_descs", Model.Descs);
                    Parameters.Add("p_user_status", Model.UserStatus);
                    Parameters.Add("p_file_name", Model.FileName);
                    Parameters.Add("p_path_file", Model.PathFile);
                    Parameters.Add("p_user_edit", Model.UserEdit);
                    Parameters.Add("p_lastupdatestamp", 0);
                    var dd = conn.Query<dynamic>("fmm_workshop_user_u", Parameters, commandType: CommandType.StoredProcedure).ToList();
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
