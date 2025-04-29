using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RabbitMqListenerEmailSenderService
{
    public static class EmailSender
    {
        public static void ProcessAndSendEmails(Dictionary<string, object> emailData, string dbType)
        {
            try
            {
                if (!IsValidEmailData(emailData, out int emailId, out string recipient, out string subject, out string body, out string jsonData))
                {
                    UpdateEmailStatus(dbType, emailId, 5); // JSON Hatalı (Failed)
                    return;
                }

                string finalBody = PrepareEmailBody(dbType, emailId, body, jsonData);

                if (string.IsNullOrWhiteSpace(finalBody))
                {
                    Console.WriteLine($"⚠️ Email ID {emailId}: HTML tablo oluşturulamadı.");
                    UpdateEmailStatus(dbType, emailId, 5);
                    return;
                }

                if (SendEmailWithRetry(recipient, subject, finalBody))
                {
                    Console.WriteLine($"✅ Email ID {emailId} başarıyla gönderildi.");
                    UpdateEmailStatus(dbType, emailId, 2); // Sent
                }
                else
                {
                    Console.WriteLine($"❌ Email ID {emailId} gönderilemedi. Retrying...");
                    UpdateEmailStatus(dbType, emailId, 4); // Retrying
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email işlenirken hata oluştu: {ex.Message}");
            }
        }

        private static bool IsValidEmailData(Dictionary<string, object> emailData, out int emailId, out string recipient, out string subject, out string body, out string jsonData)
        {
            emailId = emailData.ContainsKey("@EmailId") ? Convert.ToInt32(emailData["@EmailId"]) : 0;

            recipient = emailData.TryGetValue("@Recipient", out var recipientObj) ? recipientObj?.ToString() ?? "" : "";
            subject = emailData.TryGetValue("@Subject", out var subjectObj) ? subjectObj?.ToString() ?? "" : "";
            body = emailData.TryGetValue("@Body", out var bodyObj) ? bodyObj?.ToString() ?? "" : "";
            jsonData = emailData.TryGetValue("@JsonData", out var jsonObj) ? jsonObj?.ToString() ?? "" : "";

            if (string.IsNullOrWhiteSpace(recipient) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(jsonData))
            {
                Console.WriteLine($"⛔ Email ID {emailId}: Eksik email alanları tespit edildi.");
                return false;
            }

            if (!Utils.IsValidJson(jsonData))
            {
                Console.WriteLine($"⚠️ Email ID {emailId}: JSON verisi hatalı.");
                return false;
            }

            return true;
        }


        private static string PrepareEmailBody(string dbType, int emailId, string body, string jsonData)
        {
            int studentId = GetStudentIdFromJson(jsonData);
            string picturePath = SQLUtils.GetStudentPicturePath(dbType, studentId);
            string htmlData = Utils.GenerateHtmlTable(jsonData);

            if (!string.IsNullOrWhiteSpace(picturePath))
            {
                body += $"<br><br><img src='{picturePath}' width='200' height='200' />";
            }

            if (string.IsNullOrWhiteSpace(htmlData))
            {
                Console.WriteLine($"⚠️ Email ID {emailId}: HTML tablo oluşturulamadı.");
                return null;
            }

            return body + "<br><br>" + htmlData;
        }

        private static bool SendEmailWithRetry(string recipient, string subject, string finalBody, int retryCount = 3)
        {
            for (int i = 0; i < retryCount; i++)
            {
                string result = Utils.SendEmail(recipient, subject, finalBody);

                if (result == "OK")
                    return true;

                Console.WriteLine($"⚠️ Email gönderme denemesi başarısız (Deneme {i + 1}/{retryCount})");
                Thread.Sleep(2000); // 2 saniye bekleme
            }

            return false;
        }

        public static void RetryFailedEmails(string dbType)
        {
            foreach (int status in new[] { 1, 4 }) // 1: İşlemde kalan, 4: Başarısız olan
            {
                DataTable emails = GetEmailsByStatus(dbType, status);

                foreach (DataRow row in emails.Rows)
                {
                    int emailId = Convert.ToInt32(row["Id"]);
                    string jsonData = row["JsonData"].ToString();

                    switch (status)
                    {
                        case 1: // İşlemde kalan mailler
                            if (!Utils.IsValidJson(jsonData))
                            {
                                Console.WriteLine($"❌ Email ID {emailId}: JSON Hatalı! ({dbType})");
                                UpdateEmailStatus(dbType, emailId, 5); // JSON hatalıysa tekrar denenmesin
                            }
                            else
                            {
                                Console.WriteLine($"✅ Email ID {emailId}: Tekrar kuyruğa alındı! ({dbType})");
                                UpdateEmailStatus(dbType, emailId, 0); // Tekrar işleme al (Queued)
                            }
                            break;

                        case 4: // Başarısız olan mailler
                            if (!Utils.IsValidJson(jsonData))
                            {
                                Console.WriteLine($"❌ Email ID {emailId}: JSON Hatalı! ({dbType})");
                                UpdateEmailStatus(dbType, emailId, 5); // JSON hatalıysa tekrar denenmesin
                            }
                            else
                            {
                                Console.WriteLine($"✅ Email ID {emailId}: Tekrar kuyruğa alındı! ({dbType})");
                                UpdateEmailStatus(dbType, emailId, 0); // Tekrar işleme al (Queued)
                            }
                            break;

                        default:
                            Console.WriteLine($"⚠️ Bilinmeyen email durumu: {status} (Email ID: {emailId})");
                            break;
                    }
                }
            }
        }

        // 📌 **JSON'dan Student ID Çekme Metodu**
        private static int GetStudentIdFromJson(string jsonData)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonData);
                return jsonObject.ContainsKey("StudentId") ? jsonObject["StudentId"].Value<int>() : -1;
            }
            catch
            {
                return -1;
            }
        }

        private static DataTable GetEmailsByStatus(string dbType, int status) =>
            dbType == "SQL" ? SQLUtils.GetSqlEmailsByStatus(status) : SQLUtils.GetPostgreEmailsByStatus(status);

        // ✅ HATA BURADA: ÇAKIŞAN METOTLARI AYIRDIK
        private static void UpdateEmailStatus(string dbType, int emailId, int status)
        {
            if (dbType == "SQL")
            {
                SQLUtils.UpdateSqlEmailStatus(emailId, status);
            }
            else if (dbType == "PostgreSQL")
            {
                SQLUtils.UpdatePostgreEmailStatus(emailId, status);
            }
            else
            {
                throw new ArgumentException($"Geçersiz veritabanı tipi: {dbType}");
            }
        }
    }
}
