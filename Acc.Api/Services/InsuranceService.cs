using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class InsuranceService
    {
        IConfiguration config;
        private IHostingEnvironment _environment;
        private FunctionString fn;
        private InsuranceRepo repoInsurance;

        public InsuranceService(IConfiguration Configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            config = Configuration;
            repoInsurance = new InsuranceRepo(Tools.ConnectionString(Configuration));
            fn = new FunctionString(Tools.ConnectionString(Configuration));
        }

        public void ValidasiData(UploadFileInsurance DataUpload)
        {
            try
            {
                DataUpload.vendor_token = fn.DecryptString(DataUpload.vendor_token);
                var DataInsurance = repoInsurance.GetVendorInsurance(DataUpload.vendor_token);
                if (DataInsurance == null)
                {
                    throw new Exception("Vendor Token Not Valid.");
                }

                if (string.IsNullOrEmpty(DataUpload.insurance_policy_no))
                {
                    throw new Exception("Insurance Policy No could not be empty.");
                }
                if (string.IsNullOrEmpty(DataUpload.order_no))
                {
                    throw new Exception("Order No could not be empty.");
                }

                string parameter = string.Format("order_no = '{0}'",DataUpload.order_no);
                var CntData = Convert.ToInt32(fn.SelectScalar(Enum.SQL.Function.Aggregate.Count, "op_order", "order_no", parameter));
                if (CntData == 0)
                {
                    throw new Exception("SOT Not Valid.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateOrder(string FileName,string FilePath,string SOT,string PoliceNo)
        {
            bool result = false;
            try
            {
                result = repoInsurance.UpdateOpOrder(FileName, FilePath, SOT, PoliceNo);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool InsertLog(VendorInsuranceLog param)
        {
            bool result = false;
            try
            {
                result = repoInsurance.InsertLog(param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}
