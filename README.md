# api-clients
Zebpay rest client and Zebpay Model will also be available as nuget (As of now its private nuget). http://zebnugetserver-dev.azurewebsites.net/nuget

<h1>How to Install</h1>

`PM> Install-Package Zebpay.RestClient`

<h1>How to Use</h1>

- <i>Generate your key at https://dev.zebpay.co/applications (Web or Console/backed) to generate clientid, clientsecret, scope, and apiSecret.</i>
- <i>Subscribe to our API product for Professional at https://dev.zebpay.co/products to generate Zebpay Subscription Key.</i>

<h2>1. Configure all values as app setting</h2>

````
{
  "ZebpaySettings": {
    "BaseApiUrl": "https://www.zebpay.co/api/v1",
    "BaseAuthUrl": "https://login.zebpay.co/",

    "ClientId": "cbb35552-1d7a-4b0f-a45c-8fcee6342f93", //TODO: Change the value to your actual ClientId
    "ClientSecret": "8876cc4a-12aa-4c17-888f-fc99b8e53d62", //TODO: Change the value to your actual ClientSecret
    "SignatureSecret": "5afdf23c-0118-40d0-a2b8-86985b149c28", //TODO: Change the value to your actual SignatureSecret
    "ZebpaySubscriptionKey": "e6c8a7092f6b4a8e9dc3c7544827715f", //TODO: Change the value to your actual zebpay subscription key
    "AllowedScopes": "openid profile wallet:transactions:read trade:read trade:create",

    //Configure AccessToken & RefreshToken only you have already retrieved from Zebpay Auth Server
    "AccessToken": "", //TODO: Generate user token and paste here
    "RefreshToken": "" //TODO: Generate refresh token and paste here
  }
}    
````


<h2>2. Initialize Zebpay Client</h2>

````
            var zebpayRestClient = new Zebpay.RestClient.ZebClient(Configuration, userToken, refreshToken);    
````

<h2>3. Login to receive an access token, use below methods in a sequential way</h2>

````
            var loginRequest = new Login { country_code = countryCode, mobile_number = mobileNumber, client_id = clientId, client_secret = clientSecret };
            var loginResponse = zebpayRestClient.Login(loginRequest).Result;

            var verifyOtpRequest = new OtpVerification { otp = otp, verification_code = loginResponse.Data.verification_code, client_id = clientId, client_secret = clientSecret };
            var otpResponse = zebpayRestClient.VerifyOTP(verifyOtpRequest).Result;

            var verifyPinRequest = new PinVerification { pin = pin, scope = zebpayApiScopes, grant_type = "user_credentials", verification_code = otpResponse.Data.verification_code, client_id = clientId, client_secret = clientSecret };
            var authResponse = zebpayRestClient.VerifyPin(verifyPinRequest).Result;

            userToken = authResponse.Data.access_token;
            refreshToken = authResponse.Data.refresh_token; //you will get this only if you are allowed for offline access   
````

<h2>4. Reinitialize our restclient with  usertoken now</h2>

````
            zebpayRestClient = new Zebpay.RestClient.ZebClient(Configuration, userToken, null);     
````

<h2>5. Call any API just by using our helper methods</h2>

````
            var walletBalanceRequest = new WalletBalance { currency = currency };
            var walletBalanceResponse = zebpayRestClient.WalletBalance(walletBalanceRequest).Result;
            
            var bidRequest = new Bid { trade_pair = tradepair, size = size, price = price };
            var bidResponse = zebpayRestClient.Bid(bidRequest).Result;
            
            var orderCancelRequest = new CancelOrder { order_id = orderId, trade_pair = tradepair };
            var orderCancelResponse = zebpayRestClient.Cancel(orderCancelRequest).Result;
            
            var askRequest = new Ask { trade_pair = tradepair, size = size, price = price };
            var askResponse = zebpayRestClient.Ask(askRequest).Result;
````
