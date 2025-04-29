using System.Configuration;


namespace RabbitMqListenerEmailSenderService
{
    public static class ConnectionStrinManager
    {
        public static string etDecryptedConnectionString()
        {
            string encryptedConnectionString = ConfigurationManager.ConnectionStrings["MsSqlConnection"].ConnectionString;

            string decryptedConnectionString = SecurityHelper.Decrypt(encryptedConnectionString);

            return decryptedConnectionString;
        }
    }


}
