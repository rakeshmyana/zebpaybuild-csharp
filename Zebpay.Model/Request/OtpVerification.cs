namespace Zebpay.Model.Request
{
    public class OtpVerification
    {
        public string otp { get; set; }
        public string verification_code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
