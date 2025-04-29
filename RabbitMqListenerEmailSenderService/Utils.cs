using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace RabbitMqListenerEmailSenderService
{
    public static partial class Utils
    {
        private static readonly string _sqlConnectionString = ConnectionStrinManager.etDecryptedConnectionString();

        public static string SendEmail(string recipient, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("akinincecik@gmail.com", "azqnnxemtunebway");

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("akinincecik@gmail.com"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipient);

                    // Spam filtrelerini azaltmak için başlıklar
                    mailMessage.Headers.Add("X-Priority", "1");
                    mailMessage.Headers.Add("X-MSMail-Priority", "High");
                    mailMessage.Headers.Add("Importance", "High");

                    client.Send(mailMessage);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static string GenerateHtmlTable(string jsonData)
        {
            using (SqlConnection conn = new SqlConnection(_sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetHtmlTemplate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JsonData", jsonData);
                    cmd.Parameters.Add("@Html", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return cmd.Parameters["@Html"].Value.ToString();
                }
            }
        }

        public static string GenerateHtmlTable(JObject jsonData)
        {
            string jsonString = jsonData.ToString();
            return GenerateHtmlTable(jsonString);
        }

        private static string GetBase64Image(string picturePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(picturePath))
                {
                    Console.WriteLine("⚠️ Resim yolu boş. Varsayılan resim döndürülüyor.");
                    return "data:image/png;base64," + DefaultProfileBase64();
                }

                // 📌 Gelen `picturePath` değerini kontrol edelim!
                Console.WriteLine($"🔍 Gelen picturePath: {picturePath}");

                // Eğer path yanlışsa düzeltelim
                if (picturePath.StartsWith("Resources"))
                {
                    Console.WriteLine("❌ Yanlış resim yolu geldi! Varsayılan resim döndürülüyor.");
                    return "data:image/png;base64," + DefaultProfileBase64();
                }

                // Test için tam bir resim yolu tanımla
                string fullPath = picturePath;
                Console.WriteLine($"🔍 Oluşturulan fullPath: {fullPath}");

                // Dosya var mı kontrol et
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"❌ Resim bulunamadı: {fullPath}. Varsayılan resim yüklenecek.");
                    return "data:image/png;base64," + DefaultProfileBase64();
                }

                // ✅ Dosya bulundu mesajı
                Console.WriteLine("✅ Dosya bulundu! Base64 formatına dönüştürülüyor...");

                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string base64String = Convert.ToBase64String(imageBytes);
                string mimeType = GetMimeType(fullPath);
                string base64Image = $"data:{mimeType};base64,{base64String}";

                // ✅ Base64 dönüşümü başarılı mı?
                Console.WriteLine(base64Image.StartsWith("data:image/jpeg;base64,") || base64Image.StartsWith("data:image/png;base64,")
                    ? "✅ Resim başarıyla dönüştürüldü!"
                    : "❌ Base64 dönüşüm hatası!");

                return base64Image;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Resim yükleme hatası: {ex.Message}. Varsayılan resim yüklenecek.");
                return "data:image/png;base64," + DefaultProfileBase64();
            }
        }
        private static string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".webp":
                    return "image/webp";
                default:
                    return "image/png"; // Varsayılan PNG
            }
        }

        private static string DefaultProfileBase64()
        {
            try
            {
                string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icons8-person-80.png");

                if (!File.Exists(defaultImagePath))
                {
                    Console.WriteLine($"❌ Varsayılan resim bulunamadı: {defaultImagePath}");
                    return "";
                }

                byte[] imageBytes = File.ReadAllBytes(defaultImagePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Varsayılan resim yükleme hatası: {ex.Message}");
                return "";
            }
        }

        public static bool IsValidJson(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
                return false;

            try
            {
                JToken.Parse(jsonData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
