using System;

namespace Zebpay.Model.Response
{
    public class Balance
    {
        public string currency { get; set; }
        public decimal balance { get; set; }
        public Int64 last_updated { get; set; }
        public decimal pending_trade_balance { get; set; }
    }
}
