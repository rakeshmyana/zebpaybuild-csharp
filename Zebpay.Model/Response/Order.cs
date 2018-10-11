namespace Zebpay.Model.Response
{
    public class Order
    {
        public int id { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public string type { get; set; } = "limit";
        public string trade_pair { get; set; }
        public string side { get; set; }
        public long created_at { get; set; }
        public decimal executed_value { get; set; }
        public decimal cancel_value { get; set; }
        public decimal fill_fees { get; set; }
        public decimal filled_size { get; set; }
        public string status { get; set; }
        public bool settled { get; set; }
    }
}
