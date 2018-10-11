namespace Zebpay.RestClient.Utility.Constants
{
    internal class Urls
    {
        #region Auth

        internal const string Login = "/user/login";
        internal const string VerifyOtp ="/user/verifyotp";
        internal const string VerifyPin = "/user/verifypin";
        internal const string Logout = "/user/logout";
        internal const string Profile = "/user/profile";

        #endregion

        #region trade

        internal const string CreateOrder = "/orders";
        internal const string GetOrders = "/orders?trade_pair={0}&order_id={1}&status={2}&page={3}&limit={4}";
        internal const string CancelOrder = "/orders/{0}";
        internal const string OrderDetail = "/orders/{0}/fills";
        internal const string Tradebalance = "/orders/{0}/balance";

        #endregion
    }
}
