using System;

namespace Zebpay.Model.Response
{
    public class Fills
    {
        public decimal Size { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string FeesType { get; set; }
        public decimal Fees { get; set; }
        public decimal IntradayFees { get; set; }
        public decimal TotalFees { get; set; }
        public string FeeCurrency { get; set; }
        public long CreateDate { get; set; }
    }
}
