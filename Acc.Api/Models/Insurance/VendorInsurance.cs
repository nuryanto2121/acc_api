using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class VendorInsurance
    {
        public int cm_vendor_insurance_id {get;set;}
        public string vendor_code{get;set;}
        public string vendor_name {get;set;}
        public string merchant_id{get;set;}
        public string secret_key {get;set;}
        public string url{get;set;}
        public string merchant_id_uat {get;set;}
        public string secret_key_uat{get;set;}
        public string url_uat {get;set;}
        public bool is_prod {get;set;}
    }
    public class VendorInsuranceLog
    {
        public string vendor_token{get; set;}
        public string order_no{get; set;}
        public string insurance_policy_no{get; set;}
        public string file_name{get; set;}
        public string path_file{get; set;}
        public string ip_address{get; set;}
        public string user_agent{get; set;}
        public string user_input{get; set;}
        public string param_string{get;set;}
    }

    /// <summary>
    /// 
    /// </summary>
    public class CMVendorInsurance
    {
        public int cm_vendor_insurance_id{get; set;}
        public string vendor_code{get; set;}
        public string insurance_policy_no{get; set;}
        public string file_name{get; set;}
        public string path_file{get; set;}
        public string ip_address{get; set;}
        public string user_agent{get; set;}
        public string user_input{get; set;}
    }
}
