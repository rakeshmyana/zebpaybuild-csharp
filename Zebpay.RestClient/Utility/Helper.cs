using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Zebpay.Model;

namespace Zebpay.RestClient.Utility
{
    public static class Helper
    {
        private static string[] restrictHeader = { "client_secret", "refresh_token", "APISecretKey" };
        public static ResponseModel<T> CallService<T>(dynamic data, string url, IDictionary<string, string> headers, Method requestMethod = Method.POST)
        {
            if(data == null)
            {
                data = new Object();
            }
            ConfigureSecurityHeaders(headers, url, data);
            var client = new RestSharp.RestClient(url);

            var request = new RestRequest(requestMethod);
            if (request.Method != Method.GET)
            {
                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
            }
            foreach (var header in headers.Where(h => !restrictHeader.Contains(h.Key)))
            {
                request.AddHeader(header.Key, header.Value);
            }
            var response = client.Execute<ResponseModel<T>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ValidateSecurityHeaders(response, headers);
                var result = JsonConvert.DeserializeObject<ResponseModel<T>>(response.Content);
                return result;
            }
            return new ResponseModel<T> { StatusCode = (int)response.StatusCode, StatusDescription = response.StatusDescription };
        }

        private static void ValidateSecurityHeaders<T>(IRestResponse<ResponseModel<T>> response, IDictionary<string, string> headers)
        {
            var signatureInHeader = response.Headers.First(h => h.Name == "x-hmac").Value.ToString();

            var sig1 = GenerateHash(headers["APISecretKey"], response.Content);
            if(signatureInHeader != sig1)
            {
                //throw new AccessViolationException();
            }
        }

        private static void ConfigureSecurityHeaders(IDictionary<string, string> headers, string url = null, dynamic requestBody = null)
        {
            var currentTimestamp = DateTime.UtcNow.Ticks.ToString();
            headers.Add("timestamp", currentTimestamp);
            var requestId = Guid.NewGuid().ToString();
            headers.Add("RequestId", requestId);
            var hmac = CalculateSignature(url, requestBody, currentTimestamp, headers["client_id"], headers["APISecretKey"]);
            headers.Add("ApiSignature", hmac);
        }

        private static string CalculateSignature(string url, dynamic requestBody, string timestamp, string clientId, string apiSecretKey)
        {
            var messageBody = JsonConvert.SerializeObject(requestBody);
            var headersInpayLoad = "client_Id:" + clientId;

            var payLoad = Method.POST + "\n" + timestamp +
                "\n" + url + "\n" + messageBody + "\n";
            payLoad = payLoad + headersInpayLoad;
            return GenerateHash(apiSecretKey, payLoad);
        }

        private static string GenerateHash(string apiSecretKey, string payLoad)
        {
            var hmacSha256 = new HMACSHA256 { Key = Encoding.UTF8.GetBytes(apiSecretKey) };
            var hashPayLoad = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(payLoad));
            return Convert.ToBase64String(hashPayLoad);
        }
    }
}
