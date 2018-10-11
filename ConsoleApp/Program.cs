using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Zebpay.Model.Request;
using Zebpay.RestClient;

namespace ZebClient.Consumer.ConsoleApp
{
    class Program
    {
        private static IConfiguration Configuration;
        private static string zebpaySubscriptionKey;
        public static string zebpayApiScopes;
        private static string accessToken;
        private static string refreshToken;
        public static string clientId;
        public static string clientSecret;
        private static string apiSecret;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");
            Logout();
            Environment.Exit(-1);
            return true;
        }
        #endregion

        static void Main(string[] args)
        {
            // Some boilerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            Configuration = configuration;

            Console.WriteLine("+----------------------------------+");
            Console.WriteLine("|       Welcome to Zebpay API      |");
            Console.WriteLine("+----------------------------------+");
            Console.WriteLine("");
            Console.WriteLine("You have to be a valid Zebpay user to access Zebpay data. \n" +
                "Please make sure you have a valid ClientId, ClientSecret, ApiSecret, ZebpaySubscriptionKey, AllowedScopes configured in 'appsettings.json' file, before you continue. " +
                "This is mandatory for all the below sign-in methods.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Please key-in the way you want to sign-in] :: ");
            Console.WriteLine("\t1 : Sign-in using Zebpay Login API :- ");
            Console.WriteLine("\t    You can only use your own account related activities here.");
            Console.WriteLine("\t2 : Sign-in using Zebpay Credentials via browser :- ");
            Console.WriteLine("\t    You can ask any Zebpay user to use their login here, and you can access any User's Zebpay data.");
            Console.WriteLine("\t3 : Use Access token and/or refresh token from Configuration file :- ");
            Console.WriteLine("\t    Please make sure you aready have user's access token in appsettings.json file.");

            Console.WriteLine("\t4 : exit");

            clientId = configuration["ZebpaySettings:ClientId"];
            clientSecret = configuration["ZebpaySettings:ClientSecret"];
            apiSecret = configuration["ZebpaySettings:ApiSecret"];
            zebpayApiScopes = configuration["ZebpaySettings:AllowedScopes"];
            zebpaySubscriptionKey = configuration["ZebpaySettings:ZebpaySubscriptionKey"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(apiSecret) || string.IsNullOrEmpty(zebpayApiScopes) || string.IsNullOrEmpty(zebpaySubscriptionKey))
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("ERROR : " +
                    "Please make sure you have a valid ClientId, ClientSecret, ApiSecret, AllowedScopes and ZebpaySubscriptionKey" +
                    " in 'application.json' file before you continue. " +
                    "This is mandatory for all the sign-in methods.");
                Console.WriteLine("");
                Console.WriteLine("Press any key to exit..");
                Console.ReadLine().ToLower();
                return;
            }

            var userResponse = Console.ReadLine().ToLower();

            switch (userResponse)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    Console.WriteLine("Information : " +
                    "In this example we are using 'http://127.0.0.1:4999' as a RedirectUri. " +
                    "Please make sure you have configured the ClientRedirect URL correctly for your application. ");
                    Console.WriteLine("");
                    Console.WriteLine("Are you sure, you have configured it correctly and want to proceed ?? Y/YES");
                    userResponse = Console.ReadLine().ToLower();

                    if (userResponse != "y" && userResponse != "yes")
                    {
                        Console.WriteLine("Thank You");
                        Console.WriteLine("Press Any Key to Exit....");
                        Console.ReadKey();
                        return;
                    }
                    var authResponse = AuthHelperBrowserSignIn.SignIn(Configuration);
                    accessToken = authResponse.access_token;
                    refreshToken = authResponse.refresh_token;
                    break;
                case "3":
                    Console.WriteLine("Information : " +
                    "Please make sure you aready have user's access token in appsettings.json file. ");
                    Console.WriteLine("");
                    Console.WriteLine("Are you sure, you have configured it correctly and want to proceed ?? Y/YES");
                    userResponse = Console.ReadLine().ToLower();

                    if (userResponse != "y" && userResponse != "yes")
                    {
                        Console.WriteLine("Thank You");
                        Console.WriteLine("Press Any Key to Exit....");
                        Console.ReadKey();
                        return;
                    }
                    accessToken = configuration["ZebpaySettings:AccessToken"];
                    refreshToken = configuration["ZebpaySettings:RefreshToken"];
                    break;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                //Invalid Login - or login failed
                Console.WriteLine("Invalid Login - or login failed");
                Console.WriteLine("Press any key to exit..");
                Console.ReadKey();
                return;
            }

            CallAPI();
        }

        private static void CallAPI()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Thank you for Sign in - you can call below API now :: -- ");
            Console.WriteLine("");
            Console.WriteLine("");
            var invalidAttempt = 0;
            var repeatActions = true;
            while (repeatActions)
            {
                Console.WriteLine("Please key-in the number for the action you want to perform :: ");
                Console.WriteLine("\t1 : Get User Profile Data");
                Console.WriteLine("\t2 : Get User Balance by Tradepair");
                Console.WriteLine("\t3 : List All Orders");
                Console.WriteLine("\t4 : Get Order Details(Fill's)");
                Console.WriteLine("\t5 : Place a new Order - Bid Request");
                Console.WriteLine("\t6 : Place a new Order - Ask Request");
                Console.WriteLine("\t7 : Cancel a pending order");

                Console.WriteLine("\t99 : Feeling Good : Exit");

                var userResponse = Console.ReadLine().ToLower();

                switch (userResponse)
                {
                    case "1":
                        GetUserProfile();
                        break;
                    case "2":
                        GetTradepairBalance();
                        break;
                    case "3":
                        ListOrders();
                        break;
                    case "4":
                        Fills();
                        break;
                    case "5":
                        Bid();
                        break;
                    case "6":
                        Ask();
                        break;
                    case "7":
                        Cancel();
                        break;

                    case "exit":
                    case "99":
                        repeatActions = false;
                        break;

                    default:
                        if (invalidAttempt > 3)
                        {
                            repeatActions = false;
                            Console.WriteLine("Too many invalid attempts. Self Exit...");
                        }
                        else
                        {
                            invalidAttempt++;
                            Console.WriteLine("Invalid input, please try again.");
                        }
                        break;
                }
            }

            Console.WriteLine("Thank you for using Zebpay Rest API's. Press enter to exit!!");
            Console.ReadLine();
        }

        private static void Login()
        {
            Console.WriteLine("Please key-in your Country Code (Ex: MT for Malta, IN for India)");
            var countryCode = Console.ReadLine().ToLower();

            Console.WriteLine("Please key-in your mobile number, without country code.");
            var mobileNumber = Console.ReadLine().ToLower();
            var response = InitZebClient().Login(countryCode, mobileNumber).Result;

            if (string.IsNullOrEmpty(response.Data.verification_code))
            {
                Console.WriteLine(response.Data.Message);
                Console.WriteLine("Please Try again");
                return;
            }

            Console.WriteLine("Please enter one time password, you would have received on your mobile number " + mobileNumber);
            var otp = Console.ReadLine().ToLower();
            response = InitZebClient().VerifyOTP(otp, response.Data.verification_code).Result;

            if (string.IsNullOrEmpty(response.Data.verification_code))
            {
                Console.WriteLine(response.Data.Message);
                Console.WriteLine("Please Try again");
                return;
            }

            Console.WriteLine("Please enter your zebpay PIN for your Zebpay account with " + mobileNumber);
            var pin = Console.ReadLine().ToLower();
            var authResponse = InitZebClient().VerifyPin(pin, response.Data.verification_code).Result;

            if (string.IsNullOrEmpty(authResponse.Data?.access_token))
            {
                Console.WriteLine(authResponse.Data?.Message ?? authResponse.StatusDescription);
                Console.WriteLine("Please Try again");
                return;
            }

            Console.WriteLine("Login " + authResponse.StatusDescription);
            accessToken = authResponse.Data.access_token;
            refreshToken = authResponse.Data.refresh_token;

            Console.WriteLine("----------------------------------------------");
        }

        private static void Logout()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            var response = InitZebClient().Logout(accessToken, refreshToken).Result;
        }

        private static void GetUserProfile()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            var response = InitZebClient().Profile().Result;
            Console.WriteLine("API Call= " + response.StatusDescription);
            Console.WriteLine("User Name : " + response.Data.Name + " Country : " + response.Data.CountryCode);
            Console.WriteLine("----------------------------------------------");
        }

        private static void GetTradepairBalance()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            Console.WriteLine("Please key-in the tradepair for which you want to get balance");
            var tradePair = Console.ReadLine().ToLower();
            var response = InitZebClient().Balance(tradePair).Result;
            Console.WriteLine("API Balance = " + response.StatusDescription);
            if (response.StatusDescription.ToLower() == "success")
            {
                foreach (var item in response.Data)
                {
                    Console.WriteLine("Virtual Currency : " + item.currency + " Balance : " + item.balance);
                }
            }
            Console.WriteLine("----------------------------------------------");
        }

        private static void ListOrders()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            string tradePair = null;
            int orderId = 0;
            string status = "all";
            int page = 1;
            int limit = 10;
            Console.WriteLine("Do you want to apply filters, like TradePair, OrderId, Page, limit.. Y/Yes");
            var userResponse = Console.ReadLine()?.ToLower();

            if (userResponse != null && (userResponse == "y" || userResponse == "yes"))
            {
                Console.WriteLine("Tradepair = ?? : just press enter/return key to ignore this filter");
                tradePair = Console.ReadLine();

                Console.WriteLine("OrderId = ?? : just press enter/return key to ignore this filter");
                orderId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Order Status = ?? : Ex: COmpleted, Cancel, Pending, : just press enter/return key to ignore this filter");
                status = Console.ReadLine();

                Console.WriteLine("Page Number = ?? : just press enter/return key to ignore this filter");
                page = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Limit (number of records) = ?? : just press enter/return key to ignore this filter");
                limit = Convert.ToInt32(Console.ReadLine());
            }
            var response = InitZebClient().Orders(tradePair, orderId, status, page, limit).Result;
            Console.WriteLine("GetOrders " + response.StatusDescription);
            if (response.StatusDescription.ToLower() == "success")
            {
                foreach (var item in response.Data)
                {
                    Console.WriteLine("order id : " + item.id + " Size : " + item.size + " Price : " + item.price + " executed Value : " + item.executed_value + " cancelled Value : " + item.cancel_value + " status : " + item.status);
                }
                Console.WriteLine("----------------------------------------------");
            }
        }

        private static void Ask()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            Console.WriteLine("Please key-in tradepair for which you want to place ask order ");
            var tradepair = Console.ReadLine();
            Console.WriteLine("Please key-in the rate on which you want to place ask order ");
            var price = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Please key-in the size for which you want to place ask order ");
            var size = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Your total quantity of " + tradepair.Split('-')[0] + " is : " + size);
            Console.WriteLine("Placing Ask Order now... ");
            var askRequest = new OrderRequest { trade_pair = tradepair, size = size, price = price, side = "ask" };
            var response = InitZebClient().Create(askRequest).Result;
            Console.WriteLine("Trade Ask " + response.StatusDescription);
            if (response.StatusDescription.ToLower() == "success")
            {
                Console.WriteLine("Order id : " + response.Data.id + " price : " + response.Data.price + " size : " + response.Data.size);
            }
            Console.WriteLine("----------------------------------------------");
        }

        private static void Bid()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            Console.WriteLine("Please key-in tradepair for which you want to place Bid order ");
            var tradepair = Console.ReadLine();
            Console.WriteLine("Please key-in the rate on which you want to place Bid order ");
            var price = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Please key-in the size for which you want to place Bid order ");
            var size = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Your total quantity of " + tradepair.Split('-')[0] + " is : " + size);
            Console.WriteLine("Placing Bid Order now... ");
            var bidRequest = new OrderRequest { trade_pair = tradepair, size = size, price = price, side = "bid" };
            var response = InitZebClient().Create(bidRequest).Result;
            Console.WriteLine("Trade BID " + response.StatusDescription);
            if (response.StatusDescription.ToLower() == "success")
            {
                Console.WriteLine("Order id : " + response.Data.id + " price : " + response.Data.price + " size : " + response.Data.size);
            }
            Console.WriteLine("----------------------------------------------");
        }

        private static void Fills()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            Console.WriteLine("Please key-in order if for which you want to Get orders ");
            var orderId = Convert.ToInt32(Console.ReadLine());
            var response = InitZebClient().Fills(orderId).Result;
            Console.WriteLine("Fills API : " + response.StatusDescription);
            if (response.StatusDescription.ToLower() == "success")
            {
                foreach (var item in response.Data)
                {
                    Console.WriteLine("Amount : " + item.Amount + "Size : " + item.Size + " Fee : " + item.TotalFees);
                }
            }
            Console.WriteLine("----------------------------------------------");
        }
        
        private static void Cancel()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Please login first, before you call this API");
                return;
            }
            Console.WriteLine("Please key-in the order id which you want to cancel ");
            var orderId = Convert.ToInt32(Console.ReadLine());
            var response = InitZebClient().Cancel(orderId).Result;
            Console.WriteLine("Order Cancel " + response.StatusDescription);

            Console.WriteLine("----------------------------------------------");
        }
        
        private static Zebpay.RestClient.ZebClient InitZebClient()
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var refreshResponse = new Zebpay.RestClient.ZebClient(Configuration).RefreshedAccessToken(refreshToken);
                accessToken = refreshResponse.Data.access_token;
                refreshToken = refreshResponse.Data.refresh_token;
            }
            return new Zebpay.RestClient.ZebClient(Configuration, accessToken, refreshToken);
        }
    }
}