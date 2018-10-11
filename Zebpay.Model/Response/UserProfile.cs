namespace Zebpay.Model.Response
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string MobileNumber { get; set; }
        public string MobileNumberWithCountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }


    }
}