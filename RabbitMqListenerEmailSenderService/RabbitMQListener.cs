using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public class RabbitMQListener
    {
        private static IConnection _connection;
        private static IModel _channel;

        private readonly string _queueName = "email_queue";
        private readonly string _hostName = "localhost"; // RabbitMQ sunucu adresi
        private readonly string _username = "guest";
        private readonly string _password = "guest";

        public void StartListening(string dbType)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[📨] Received message: {message}");

                        var emailData = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

                        if (emailData != null && emailData.ContainsKey("@EmailId"))
                        {
                            int emailId = Convert.ToInt32(emailData["@EmailId"]);
                            Console.WriteLine($"[✔️] İşlem başlatılıyor, Email ID: {emailId}");

                            EmailSender.ProcessAndSendEmails(emailData, dbType);
                        }
                        else
                        {
                            Console.WriteLine("⛔ Geçersiz mesaj formatı.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Hata: {ex.Message}");
                    }

                    // Mesajı onayla
                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
                Console.WriteLine("[🔊] Kuyruk dinleniyor...");

                // Thread'i kilitlemeden açık tutuyoruz
                Task.Delay(-1).Wait();  // ✅ Sonsuz döngü yerine Task.Delay kullanıyoruz
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Dinleme başlatılamadı: {ex.Message}");
            }
        }

    }
}
