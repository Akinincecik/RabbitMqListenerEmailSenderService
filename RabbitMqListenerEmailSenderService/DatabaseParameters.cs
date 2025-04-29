using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public class DatabaseParameters
    {
        public string SqlConnectionString { get; set; }   // ✅ SQL Server Connection String
        public string PostgreConnectionString { get; set; }  // ✅ PostgreSQL Connection String

        public DatabaseParameters()
        {
            SqlConnectionString = ConfigurationManager.ConnectionStrings["MsSqlConnection"].ToString();
            PostgreConnectionString = ConfigurationManager.ConnectionStrings["PostgreSqlConnection"].ToString();
        }

        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString ?? throw new Exception($"Bağlantı dizesi bulunamadı: {name}");
        }
    }
}
