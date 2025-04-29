using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public class PostgreEmailSender
    {
        //public static void ProcessAndSendEmails()
        //{
        //    // 1. State 0 olan ilk e-posta ID'sini alıyoruz.
        //    DataTable queuedEmail = SQLUtils.GetPostgreQueuedEmails();

        //    foreach (DataRow row in queuedEmail.Rows)
        //    {
        //        int emailId = Convert.ToInt32(row["email_id"]);
        //        // 2. ID'ye ait e-posta verilerini alıyoruz.
        //        DataTable emailData = SQLUtils.GetPostgreEmailFromId(emailId);

        //        if (emailData.Rows.Count > 0)
        //        {
        //            // E-posta bilgilerini alıyoruz.
        //            DataRow emailRow = emailData.Rows[0];
        //            string recipient = emailRow["recipient"].ToString();
        //            string subject = emailRow["subject"].ToString();
        //            string body = emailRow["body"].ToString();
        //            string jsonData = emailRow["jsondata"].ToString();

        //            // JSON verisini HTML formatına dönüştürme işlemi
        //            JObject jsonObject = JObject.Parse(jsonData);
        //            string htmlJsonData = Utils.GenerateHtmlTable(jsonObject);

        //            // Mail gönderimi
        //            string result = Utils.SendEmail(recipient, subject, body + htmlJsonData);

        //            // Sonuç başarılıysa state'i 2'ye (Sent) güncelle
        //            if (result == "OK")
        //            {
        //                SQLUtils.UpdatePostgreEmailStatus(emailId, 5);
        //            }
        //            else
        //            {
        //                SQLUtils.UpdatePostgreEmailStatus(emailId, 5); // Hata varsa Failed
        //            }
        //        }
        //    }
        //}
    }
}
