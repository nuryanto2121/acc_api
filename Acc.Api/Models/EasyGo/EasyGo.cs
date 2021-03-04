using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class TokenEasyGO
    {
        [JsonProperty("gps_token")]
        public string gps_token { get; set; }
        [JsonProperty("gps_map_token")]
        public string gps_map_token { get; set; }
    }
    public partial class EasyGoAdd
    {
        [JsonProperty("op_order_id")]
        public int OpOrderID { get; set; }
        [JsonProperty("easy_go_do")]
        public EasyGoDO EasyGoDo { get; set; }
    }

    public partial class EasyGoDO
    {
        [JsonProperty("no_do")]
        public string NoDo { get; set; }

        [JsonProperty("no_sj")]
        public string NoSj { get; set; }

        [JsonProperty("car_plate")]
        public string CarPlate { get; set; }

        [JsonProperty("tgl_do")]
        public DateTime TglDo { get; set; }

        [JsonProperty("opsi_complete")]
        public long OpsiComplete { get; set; }

        [JsonProperty("dur_valid_geofence")]
        public long DurValidGeofence { get; set; }

        [JsonProperty("allow_multiple_do")]
        public long AllowMultipleDo { get; set; }

        [JsonProperty("maxtime_delivery")]
        public long MaxtimeDelivery { get; set; }

        [JsonProperty("maxtime_checking")]
        public long MaxtimeChecking { get; set; }

        [JsonProperty("alert_telegram")]
        public string AlertTelegram { get; set; }

        [JsonProperty("alert_email")]
        public string AlertEmail { get; set; }

        [JsonProperty("alert_dur_idle")]
        public long AlertDurIdle { get; set; }

        [JsonProperty("alert_dur_notUpdate")]
        public long AlertDurNotUpdate { get; set; }

        [JsonProperty("alert_dur_terlarang")]
        public long AlertDurTerlarang { get; set; }

        [JsonProperty("alert_tujuan_lain")]
        public long AlertTujuanLain { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("user_login")]
        public string UserLogin { get; set; }

        [JsonProperty("replace_coord")]
        public long ReplaceCoord { get; set; }

        [JsonProperty("checkpoint_auto_route")]
        public long CheckpointAutoRoute { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("url_reply")]
        public string UrlReply { get; set; }

        [JsonProperty("backdate_leaving_asal_minutes")]
        public long BackdateLeavingAsalMinutes { get; set; }

        [JsonProperty("driver_id")]
        public long DriverId { get; set; }

        [JsonProperty("geo_asal")]
        public List<GeoAsal> GeoAsal { get; set; }

        [JsonProperty("geo_tujuan")]
        public List<GeoTujuan> GeoTujuan { get; set; }
    }

    public partial class GeoAsal
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("radius")]
        public long Radius { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; } 

        [JsonProperty("lat")]
        public string Lat { get; set; } 

        [JsonProperty("plan_time")]
        public string PlanTime { get; set; }
    }

    public partial class GeoTujuan
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("no_sj")]
        public string NoSj { get; set; }

        [JsonProperty("radius")]
        public long Radius { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; } 

        [JsonProperty("lat")]
        public string Lat { get; set; } 

        [JsonProperty("plan_time")]
        public string PlanTime { get; set; }

        [JsonProperty("cust_alert_telegram")]
        public string CustAlertTelegram { get; set; }

        [JsonProperty("cust_alert_email")]
        public string CustAlertEmail { get; set; }

        [JsonProperty("std_km_delivery")]
        public long StdKmDelivery { get; set; }

        [JsonProperty("std_minute_delivery")]
        public long StdMinuteDelivery { get; set; }
    }
    public class ResponseDO
    {
        [JsonProperty("ResponseCode")]
        public long ResponseCode { get; set; }

        [JsonProperty("ResponseMessage")]
        public string ResponseMessage { get; set; }

        [JsonProperty("Data")]
        public DOID Data { get; set; }
    }

    public class ResponsePosition
    {
        [JsonProperty("ResponseCode")]
        public long ResponseCode { get; set; }

        [JsonProperty("ResponseMessage")]
        public string ResponseMessage { get; set; }

        [JsonProperty("Data")]
        public object Data { get; set; }
    }

    public class DOID
    {
        [JsonProperty("do_id")]
        public int DoID { get; set; }
    }


}
