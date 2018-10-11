# api-clients
Zebpay rest client and Zebpay Model will also be available as nuget.

<h1>How to Install</h1>

`PM> Install-Package Zebpay.RestClient`

<h1>How to Use</h1>

- <i>Generate your key at https://build.zebpay.co/application (Web or Console/backed) to generate clientid, clientsecret, scope, and apiSecret and zebpaysubscriptionkey.</i>

<h2>1. Configure all values as app setting</h2>

````
            {
              "ZebpaySettings": {
                //LIVE URL's -- Login to https://build.zebpay.com/ to get your own credentials
                //"BaseApiUrl": "https://www.zebapi.com/api/v1",
                //"BaseAuthUrl": "https://connect.zebpay.com/",

                //Sandbox URL's -- Login to https://build.zebpay.co/ to get your own credentials
                "BaseApiUrl": "https://www.zebpay.co/api/v1",
                "BaseAuthUrl": "https://login.zebpay.co/",

                //Credentials
                "ClientId": "xxxx-xxxx-xxx", //TODO: Change the value to your actual ClientId
                "ClientSecret": "xxxx-xxxx-xxx", //TODO: Change the value to your actual ClientSecret
                "ApiSecret": "xxxx-xxxx-xxx", //TODO: Change the value to your actual ApiSecret
                "ZebpaySubscriptionKey": "xxxx-xxxx-xxx", //TODO: Change the value to your actual zebpay subscription key
                "AllowedScopes": "xxxx-xxxx-xxx",

                //Configure AccessToken & RefreshToken only you have already have it saved, the one retrieved from Zebpay Auth Server
                "AccessToken": "", //Generate user token and paste here
                "RefreshToken": "", //Generate refresh token and paste here,

                //These settings will be used once in a lifetime for a "ClientId-User" combination, and will be ignored in all subsequent times
                "DailyTradeLimit": "500000",
                "TotalTradeLimit": "5000000"
              }
            }    
````


<h2>2. Initialize Zebpay Client</h2>

````
            var zebpayRestClient = new Zebpay.RestClient.ZebClient(Configuration, userToken, refreshToken);    
````

<h2>3. Login to receive an access token, use below methods in a sequential way</h2>

````
            var loginResponse = zebpayRestClient.Login(countryCode, mobileNumber).Result;
            var otpResponse = zebpayRestClient.VerifyOTP(otp, loginResponse.Data.verification_code).Result;
            var authResponse = zebpayRestClient.VerifyPin(pin, otpResponse.Data.verification_code).Result;

            var accessToken = authResponse.Data.access_token;
            var refreshToken = authResponse.Data.refresh_token; //you will get this only if you are allowed for offline access   
````

<h2>4. Reinitialize our restclient with  usertoken now</h2>

````
            zebpayRestClient = new Zebpay.RestClient.ZebClient(Configuration, accessToken, null);     
````

<h2>5. Call any API just by using our helper methods</h2>

````
            //Get Balance of any pair
            var tradeBalanceResponse = zebpayRestClient.Balance(tradePair).Result;
            
            //Get list of orders
            //All parameters below are optional
            var getAllOrdersresponse = zebpayRestClient.Orders(tradePair, orderId, status, page, limit).Result;
            
            //Create New Order
            //side can be ask/bid
            var createNewOrderRequest = new OrderRequest { trade_pair = tradepair, size = size, price = price, side = "ask" };
            var response = zebpayRestClient.Create(createNewOrderRequest).Result;

            //Get order details by Id (Fills)
            var orderFillsResponse = zebpayRestClient.Fills(orderId).Result;
            
            //Cancel a pending order
            var orderCancelResponse = zebpayRestClient.Cancel(orderId).Result;
````
