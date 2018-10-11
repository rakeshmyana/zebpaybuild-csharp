using System.Threading.Tasks;
using Zebpay.Model;
using Zebpay.Model.Response;

namespace Zebpay.RestClient
{
    internal interface IAuthService
    {
        Task<ResponseModel<AuthResponse>> Login(string countryCode, string mobileNumber);
        Task<ResponseModel<AuthResponse>> VerifyOTP(string otp, string verificationCode);
        Task<ResponseModel<AuthResponse>> VerifyPin(string pin, string verificationCode);
        ResponseModel<AuthResponse> RefreshedAccessToken(string refreshToken);
        Task<ResponseModel<AuthResponse>> Logout(string accessToken, string refresh_token = null);
        Task<ResponseModel<UserProfile>> Profile();
    }
}
