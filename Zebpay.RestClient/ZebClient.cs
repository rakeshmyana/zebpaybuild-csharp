using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Zebpay.Model;
using Zebpay.Model.Request;
using Zebpay.Model.Response;
using Zebpay.RestClient.Utility;
using Zebpay.RestClient.Utility.Constants;

namespace Zebpay.RestClient
{
    public class ZebClient : IZebpayClient
    {
        private IDictionary<string, string> Headers { get; set; }
        private IConfiguration Configuration { get; set; }
        private string baseApiUrl { get; set; }
        private string baseAuthUrl { get; set; }

        public ZebClient(IConfiguration configuration, string accessToken = null, string refreshToken = null)
        {
            if (configuration == null)
                return;
            Configuration = configuration;

            baseApiUrl = configuration["ZebpaySettings:BaseApiUrl"];
            baseAuthUrl = configuration["ZebpaySettings:BaseAuthUrl"];

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "cache-control", "no-cache"},
                { "Zebpay-Subscription-Key", configuration["ZebpaySettings:ZebpaySubscriptionKey"] },
                { "client_id", configuration["ZebpaySettings:ClientId"]},
                { "client_secret", configuration["ZebpaySettings:ClientSecret"]},
                { "APISecretKey", configuration["ZebpaySettings:ApiSecret"]},
                { "Authorization", "bearer " + accessToken },
                { "refresh_token", refreshToken }
            };
            Headers = headers;
        }

        public void AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                if (Headers.Keys.Contains(header.Key))
                {
                    Headers.Remove(header.Key);
                }
                Headers.Add(header);
            }
        }

        #region Auth

        public Task<ResponseModel<AuthResponse>> Login(string countryCode, string mobileNumber)
        {
            var userLoginRequest = new Login
            {
                country_code = countryCode,
                mobile_number = mobileNumber,
                client_id = Headers["client_id"],
                client_secret = Headers["client_secret"]
            };
            return Task.Run(() => Helper.CallService<AuthResponse>(userLoginRequest, baseApiUrl + Urls.Login, Headers));
        }
        public Task<ResponseModel<AuthResponse>> VerifyOTP(string otp, string verificationCode)
        {
            var verifyOTPRequest = new OtpVerification
            {
                otp = otp,
                verification_code = verificationCode,
                client_id = Headers["client_id"],
                client_secret = Headers["client_secret"]
            };
            return Task.Run(() => Helper.CallService<AuthResponse>(verifyOTPRequest, baseApiUrl + Urls.VerifyOtp, Headers));
        }

        public Task<ResponseModel<AuthResponse>> VerifyPin(string pin, string verificationCode)
        {
            var verifyPinRequest = new PinVerification
            {
                pin = pin,
                verification_code = verificationCode,
                client_id = Headers["client_id"],
                client_secret = Headers["client_secret"],
                scope = Configuration["ZebpaySettings:AllowedScopes"],
                daily_trade_limit = Convert.ToInt32(Configuration["ZebpaySettings:DailyTradeLimit"]),
                total_trade_limit = Convert.ToInt32(Configuration["ZebpaySettings:TotalTradeLimit"]),
                grant_type = "user_credentials",
            };
            return Task.Run(() => Helper.CallService<AuthResponse>(verifyPinRequest, baseApiUrl + Urls.VerifyPin, Headers));
        }

        public ResponseModel<AuthResponse> RefreshedAccessToken(string refreshToken)
        {
            var client = new HttpClient();

            var disco = client.GetDiscoveryDocumentAsync(baseAuthUrl).Result;
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenEndpoint = disco.TokenEndpoint;
            var keys = disco.KeySet.Keys;
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = new TokenClient(disco.TokenEndpoint, Headers["client_id"], Headers["client_secret"]);
            var tokenResult = tokenClient.RequestRefreshTokenAsync(refreshToken).Result;

            return new ResponseModel<AuthResponse> { Data = new AuthResponse { access_token = tokenResult.AccessToken, refresh_token = tokenResult.RefreshToken } };
        }

        public Task<ResponseModel<AuthResponse>> Logout(string accessToken, string refresh_token = null)
        {
            var logoutRequest = new Logout
            {
                access_token = accessToken,
                refresh_token = refresh_token,
                client_id = Headers["client_id"],
                client_secret = Headers["client_secret"]
            };
            return Task.Run(() => Helper.CallService<AuthResponse>(logoutRequest, baseApiUrl + Urls.Logout, Headers));
        }

        public Task<ResponseModel<UserProfile>> Profile()
        {
            return Task.Run(() => Helper.CallService<UserProfile>(null, baseApiUrl + Urls.Profile, Headers));
        }
        #endregion

        #region Trade

        public Task<ResponseModel<OrderResponse>> Create(OrderRequest createOrderRequest)
        {
            var createOrderUrl = baseApiUrl + Urls.CreateOrder;
            return Task.Run(() => Helper.CallService<OrderResponse>(createOrderRequest, createOrderUrl, Headers));
        }

        public Task<ResponseModel<IList<Order>>> Orders(string tradePair = null, int orderId = 0, string status = "all", int page = 1, int limit = 20)
        {
            var getOrderUrl = baseApiUrl + string.Format(Urls.GetOrders, tradePair, orderId, status, page, limit);
            return Task.Run(() => Helper.CallService<IList<Order>>(null, getOrderUrl, Headers, RestSharp.Method.GET));
        }

        public Task<ResponseModel<string>> Cancel(int orderId)
        {
            var cancelUrl = baseApiUrl + string.Format(Urls.CancelOrder, orderId);
            return Task.Run(() => Helper.CallService<string>(null, cancelUrl, Headers, RestSharp.Method.DELETE));
        }

        public Task<ResponseModel<IList<Balance>>> Balance(string tradePair)
        {
            var tradePairBalanceUrl = baseApiUrl + string.Format(Urls.Tradebalance, tradePair);
            return Task.Run(() => Helper.CallService<IList<Balance>>(null, tradePairBalanceUrl, Headers,RestSharp.Method.GET));
        }
        public Task<ResponseModel<IList<Fills>>> Fills(int orderId)
        {
            var orderDetailUrl = baseApiUrl + string.Format(Urls.OrderDetail, orderId);
            return Task.Run(() => Helper.CallService<IList<Fills>>(null, orderDetailUrl, Headers, RestSharp.Method.GET));
        }

        #endregion
    }
}
