using System;

namespace Zebpay.Model.Request
{
    public class PinVerification
    {
        public string grant_type { get; set; }
        public string pin { get; set; }
        public string verification_code { get; set; }
        public string scope { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public Int32 daily_trade_limit { get; set; }
        public Int32 daily_withdraw_limit { get; set; }
        public Int32 total_trade_limit { get; set; }
        public Int32 total_withdraw_limit { get; set; }
    }
}
