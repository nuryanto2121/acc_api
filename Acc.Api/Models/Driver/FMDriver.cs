
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

namespace Acc.Api.Models
{
    public partial class VmFMDriver
    {
        [JsonProperty("ss_portfolio_id")]
        public string SsPortfolioId
        {
            get; set;
        }
        [JsonProperty("Driver")]
        public FMDriver FMDriver
        {
            get; set;
        }

        [JsonProperty("DriverDocument")]
        public List<DriverDocument> DriverDocument
        {
            get; set;
        }
        [JsonProperty("user_input")]
        public string UserInput
        {
            get; set;
        }
    }

    public partial class FMDriver
    {
        [JsonProperty("fm_driver_id")]
        public int FMDriverId
        {
            get; set;
        }

        [JsonProperty("employee_id")]
        public string EmployeeId
        {
            get; set;
        }
        [JsonProperty("password")]
        public string Password
        {
            get; set;
        }
        [JsonProperty("driver_name")]
        public string DriverName
        {
            get; set;
        }

        [JsonProperty("handphone")]
        public string Handphone
        {
            get; set;
        }

        [JsonProperty("ktp")]
        public string Ktp
        {
            get; set;
        }

        [JsonProperty("npwp")]
        public string Npwp
        {
            get; set;
        }

        [JsonProperty("sim")]
        public string Sim
        {
            get; set;
        }

        [JsonProperty("sim_expiry_date")]
        public DateTime? SimExpiryDate
        {
            get; set;
        }

        [JsonProperty("fm_sim_type_id")]
        public int FmSimTypeId
        {
            get; set;
        }

        [JsonProperty("skck")]
        public string Skck
        {
            get; set;
        }

        [JsonProperty("skck_expiry_date")]
        public DateTime? SkckExpiryDate
        {
            get; set;
        }

        [JsonProperty("employee_status")]
        public string EmployeeStatus
        {
            get; set;
        }

        [JsonProperty("address")]
        public string Address
        {
            get; set;
        }

        [JsonProperty("emergency_contact_name")]
        public string EmergencyContactName
        {
            get; set;
        }

        [JsonProperty("emergency_relation")]
        public string EmergencyRelation
        {
            get; set;
        }

        [JsonProperty("emergency_phone_no")]
        public string EmergencyPhoneNo
        {
            get; set;
        }

        [JsonProperty("emergency_remarks")]
        public string EmergencyRemarks
        {
            get; set;
        }

        [JsonProperty("bank_name")]
        public string BankName
        {
            get; set;
        }

        [JsonProperty("bank_acct_no")]
        public string BankAcctNo
        {
            get; set;
        }
        
        [JsonProperty("employee_expiry_date")]
        public DateTime? EmployeeExpiryDate
        {
            get; set;
        }

        [JsonProperty("join_date")]
        public DateTime? JoinDate
        {
            get; set;
        }

        [JsonProperty("contract_end_date")]
        public DateTime? ContractEndDate
        {
            get; set;
        }

        [JsonProperty("terminate_date")]
        public DateTime? TerminateDate
        {
            get; set;
        }

        [JsonProperty("file_name")]
        public string FileName
        {
            get; set;
        }

        [JsonProperty("path_file")]
        public string PathFile
        {
            get; set;
        }
    }

    public partial class DriverDocument
    {       
        [JsonProperty("doc_type")]
        public string DocType
        {
            get; set;
        }

        [JsonProperty("doc_no")]
        public string DocNo
        {
            get; set;
        }

        [JsonProperty("doc_file_name")]
        public string DocFileName
        {
            get; set;
        }

        [JsonProperty("doc_path_file")]
        public string DocPathFile
        {
            get; set;
        }

        [JsonProperty("expiry_date")]
        public DateTime? ExpiryDate
        {
            get; set;
        }
    }
}
