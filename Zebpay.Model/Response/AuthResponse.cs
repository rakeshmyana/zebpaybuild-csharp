namespace Zebpay.Model.Response
{
    public class AuthResponse
    {
        public string verification_code { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string Message { get; set; }
        public string error { get; set; }
    }
}
