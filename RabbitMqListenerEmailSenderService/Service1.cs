using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            DatabaseParameters dbParams = new DatabaseParameters(); // ✅ Connection bilgileri burada

            int threadCount = Convert.ToInt32(ConfigurationManager.AppSettings["ThreadCount"]);

            for (int i = 0; i < threadCount; i++)
            {
                ThreadParameters threadParameters = new ThreadParameters
                {
                    ThreadName = "Th_" + i,
                    DbParams = dbParams // ✅ Database bilgilerini doğrudan buradan alacak
                };

                Thread thread = new Thread(new ParameterizedThreadStart(Worker.testMethod));
                thread.Start(threadParameters);


            }


        }

        protected override void OnStop()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("RabbitMqListenerEmailSenderService: Stop", EventLogEntryType.Error, 101, 1);
            }
        }

#if DEBUG
        public void OnDebug()
        {
            Debugger.Launch();
            OnStart(null);
        }
#endif
    }
}
