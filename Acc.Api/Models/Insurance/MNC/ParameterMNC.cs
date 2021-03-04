using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public partial class ParameterMNC
    {
        [JsonProperty("ss_portfolio_id")]
        public string SsPortfolioID
        {
            get; set;
        }
        [JsonProperty("insuranceCode")]
        public string InsuranceCode
        {
            get; set;
        }

        [JsonProperty("companyId")]
        public string CompanyId
        {
            get; set;
        }

        [JsonProperty("companyName")]
        public string CompanyName
        {
            get; set;
        }

        [JsonProperty("sot")]
        public string Sot
        {
            get; set;
        }

        [JsonProperty("vehiclePlatNo")]
        public string VehiclePlatNo
        {
            get; set;
        }

        [JsonProperty("vehicleMerk")]
        public string VehicleMerk
        {
            get; set;
        }

        [JsonProperty("vehicleModel")]
        public string VehicleModel
        {
            get; set;
        }

        [JsonProperty("driverName")]
        public string DriverName
        {
            get; set;
        }

        [JsonProperty("origin")]
        public string Origin
        {
            get; set;
        }

        [JsonProperty("destination")]
        public string Destination
        {
            get; set;
        }

        [JsonProperty("remarks")]
        public string Remarks
        {
            get; set;
        }

        [JsonProperty("tripDate")]
        public DateTime TripDate
        {
            get; set;
        }

        [JsonProperty("Package")]
        public string Package
        {
            get; set;
        }
    }
    public partial class ParameterPostMNC
    {
        [JsonProperty("merchantToken")]
        public string MerchantToken
        {
            get; set;
        }

        [JsonProperty("companyId")]
        public string CompanyId
        {
            get; set;
        }

        [JsonProperty("companyName")]
        public string CompanyName
        {
            get; set;
        }

        [JsonProperty("sot")]
        public string Sot
        {
            get; set;
        }

        [JsonProperty("vehiclePlatNo")]
        public string VehiclePlatNo
        {
            get; set;
        }

        [JsonProperty("vehicleMerk")]
        public string VehicleMerk
        {
            get; set;
        }

        [JsonProperty("vehicleModel")]
        public string VehicleModel
        {
            get; set;
        }

        [JsonProperty("driverName")]
        public string DriverName
        {
            get; set;
        }

        [JsonProperty("origin")]
        public string Origin
        {
            get; set;
        }

        [JsonProperty("destination")]
        public string Destination
        {
            get; set;
        }

        [JsonProperty("remarks")]
        public string Remarks
        {
            get; set;
        }

        [JsonProperty("tripDate")]
        public DateTime TripDate
        {
            get; set;
        }

        [JsonProperty("Package")]
        public string Package
        {
            get; set;
        }

        public static implicit operator ParameterPostMNC(ParameterMNC v)
        {
            var c = new ParameterPostMNC();
            c.CompanyId = v.CompanyId;
            c.CompanyName = v.CompanyName;
            c.Destination = v.Destination;
            c.DriverName = v.DriverName;
            c.Origin = v.Origin;
            c.Package = v.Package;
            c.Remarks = v.Remarks;
            c.Sot = v.Sot;
            c.TripDate = v.TripDate;
            c.VehicleMerk = v.VehicleMerk;
            c.VehicleModel = v.VehicleModel;
            c.VehiclePlatNo = v.VehiclePlatNo;
            return c;
        }
    }
}
