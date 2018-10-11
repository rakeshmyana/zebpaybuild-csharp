namespace Zebpay.Model.Request
{
    public class OrderResponse
    {
        public int id { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public string type { get; set; } = "limit";
        public string trade_pair { get; set; }
        public string side { get; set; }
        public long created_at { get; set; }
        public string status { get; set; }
    }
}
