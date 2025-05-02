# RabbitMQ Listener Email Sender Service

## 🇹🇷 Proje Açıklaması (Turkish)

Bu proje, RabbitMQ üzerinden alınan mesajları dinleyerek e-posta gönderen bir servis olarak geliştirilmiştir. Hedef, RabbitMQ kullanarak sistemler arası iletişim sağlamak ve bu süreci yönetmek için responsive, asenkron ve verimli bir yapı oluşturmak. Bu serviste, işlemci seviyesinde thread kontrolü sağlanarak e-posta gönderme işlemi hem hızlı hem de verimli hale getirilmiştir.

### 🎯 Proje Amacı

Projenin amacı, RabbitMQ'yu kullanarak mesaj kuyruğu yönetimi ve e-posta gönderim süreçlerini entegre etmek. Ayrıca, bu servisle birlikte, yüksek verimli bir işlem yapısı inşa etmek, sistemi kilitlemeden çalışmasını sağlamak ve iş yükünü verimli bir şekilde dağıtarak responsive bir servis oluşturmak. Kritik nokta, sadece RabbitMQ kullanımı değil, aynı zamanda işlemci seviyesinde thread kontrolünün sağlanmasıdır.

Bu proje, servislerin kurulumu ve RabbitMQ ile entegrasyonunu anlamak için geliştirilmiş bir denemedir.

### 🛠️ Özellikler

- RabbitMQ kullanarak asenkron mesaj dinleme
- Dinlenen mesajlara göre e-posta gönderme
- Thread kontrolü ile işlemci verimliliği
- Kuyruk yönetimi ile akışkan ve responsive yapı
- RabbitMQ kuyruğunda biriken mesajların verimli şekilde işlenmesi

### 🚀 Kullanılan Teknolojiler

- RabbitMQ
- .NET (veya kullanılan dilin adı)
- SMTP (E-posta gönderimi için)
- Asenkron işlem yönetimi
- Thread kontrolü

### 📦 Kurulum ve Çalıştırma

1. Bu repoyu klonlayın:
   ```bash
   git clone https://github.com/Akinincecik/RabbitMqListenerEmailSenderService.git
-----------------------------------------------------------------------------------------------

# RabbitMQ Listener Email Sender Service

## 🇬🇧 Project Description (English)

This project is developed as a service that listens to messages from RabbitMQ and sends emails based on those messages. The goal is to create a responsive, asynchronous, and efficient system using RabbitMQ for inter-system communication while managing the process effectively. In this service, thread control at the processor level is applied to ensure that email sending operations are both fast and efficient.

### 🎯 Project Purpose

The purpose of this project is to integrate message queue management and email sending processes using RabbitMQ. Additionally, this service aims to build a high-performance processing structure, ensure smooth operation without locking the system, and create a responsive service by efficiently distributing the workload. The critical point is not only the use of RabbitMQ but also the implementation of thread control at the processor level.

This project is an experiment developed to understand the setup of services and integration with RabbitMQ.

### 🛠️ Features

- Asynchronous message listening via RabbitMQ
- Sending emails based on received messages
- Thread control for processor efficiency
- Queue management for a smooth and responsive structure
- Efficient processing of messages accumulated in the RabbitMQ queue

### 🚀 Technologies Used

- RabbitMQ
- .NET (or the name of the programming language used)
- SMTP (for email sending)
- Asynchronous process management
- Thread control

### 📦 Installation and Setup

1. Clone this repository:
   ```bash
   git clone https://github.com/Akinincecik/RabbitMqListenerEmailSenderService.git

