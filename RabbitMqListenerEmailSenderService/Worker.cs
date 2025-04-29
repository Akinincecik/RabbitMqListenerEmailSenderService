using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public class Worker
    {

        private static Object _lock = new Object();
        private static DatabaseParameters _dbParams = new DatabaseParameters();  // ✅ Bağlantı bilgilerini merkezi olarak çekiyoruz.

        public static void testMethod(object arg)
        {
            ThreadParameters threadParameters = (ThreadParameters)arg;
            List<string> databases = new List<string> { "SQL"/*, "PostgreSQL"*/ }; // ✅ Veritabanı türlerini listeye alıyoruz.

            while (true)
            {
                try
                {
                    lock (_lock)
                    {
                        foreach (var dbType in databases)
                        {
                            RabbitMQListener subscriber = new RabbitMQListener();
                            subscriber.StartListening(dbType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                Thread.Sleep(20);
            }
        }

        private static void LogError(Exception ex)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("RabbitMqListenerEmailSenderService Error: " + ex.StackTrace, EventLogEntryType.Error, 101, 1);
            }
        }
    }
}
