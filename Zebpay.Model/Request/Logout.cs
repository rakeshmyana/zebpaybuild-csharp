namespace Zebpay.Model.Request
{
    public class Logout
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
