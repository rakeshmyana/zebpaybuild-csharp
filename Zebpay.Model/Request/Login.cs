namespace Zebpay.Model.Request
{
    public class Login
    {
        public string country_code { get; set; }
        public string mobile_number { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
