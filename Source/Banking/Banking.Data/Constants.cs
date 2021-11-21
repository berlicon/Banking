namespace Banking.Data
{
    public static class Constants
    {
        public const decimal ChargePercent = 2m; //2% - bank charge for conversion operations

        public const string FacebookAppId = "978625058890076";
        public const string FacebookAppSecret = "8fd8b65a1aaaff4288eae0c08d810206";
        public const string FacebookLoginUrl = "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}";
        public const string FacebookAuthorizeUrl = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}";
        public const string FacebookScope = "public_profile,email";

        public const int SmtpPort = 465;
        public const string SmtpHost = "smtp.mail.ru";
        public const string To = "info@banking.com";
        public const string Subject = "New transaction";
        public const string Message = @"Transaction:
AccountFromId: {0}
AccountTo Number: {1}
From currency : {2}
To currency: {3}
Exchange rate: {4}
Amount : {5}
Amount actual: {6}
Description: {7}";
        public const string YahooFinanceUrl = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.xchange%20where%20pair%20in%20(%22{0}{1}%22)&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        public const string ErrorToAccountNotExist = "TO Account does not exist";
        public const string ErrorFromAccountNotExist = "FROM Account does not exist";
        public const string ErrorUserNotExist = "User with such login and password does not exist.";
        public const string ErrorUserAlreadyExisted = "User with such login and password is already existed.";
        public const string ErrorInvalidLogin = "Invalid login attempt";

    }
}
