using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ChangePassword
    {
        [Required]
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("current_password")]
        public string CurrentPassword { get; set; }

        [Required]
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }

        [Required]
        [JsonProperty("confirm_password")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePortfolio
    {
        [Required]
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("ss_portfolio_id")]
        public int SsPortfolioId { get; set; }

        [JsonProperty("portfolio_name")]
        public string PortfolioName { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

    }

    public class ForgotPassword
    {
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
