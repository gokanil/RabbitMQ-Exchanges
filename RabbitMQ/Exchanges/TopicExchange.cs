using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Exchanges
{
    public static class TopicExchange
    {
        /*
         * sebze.#.tohum => başlangıcı meyve ve son key'i çekirdek olanlar. Bu durumda Key sayısı minimum 2 tane olmalı.
         * sebze.patates.* => meyve.patates keyi ile başlayan ve 3. Key'i farketmeksizin toplam 3 key'den oluşanlar.
         */
        public static void Consumer(string Key)
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.HostName = "localhost";
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "topic-exchange-test",
                                                durable: false,
                                                autoDelete: true,
                                                type: ExchangeType.Topic);

                        var queueName = channel.QueueDeclare().QueueName;

                        channel.QueueBind(queue: queueName,
                                          exchange: "topic-exchange-test",
                                          routingKey: Key);

                        channel.BasicQos(prefetchSize: 0,
                                         prefetchCount: 1,
                                         global: false);

                        var consumer = new EventingBasicConsumer(channel);
                        channel.BasicConsume(queue: queueName,
                                             autoAck: false,
                                             consumer: consumer);

                        consumer.Received += (model, argument) =>
                        {
                            string message = Encoding.UTF8.GetString(argument.Body.ToArray());
                            Console.WriteLine();
                            Console.WriteLine(message);
                            channel.BasicAck(deliveryTag: argument.DeliveryTag, multiple: false);
                        };

                        Console.WriteLine($"'Consumer' başlatıldı. \"topic-exchange-test\" isimli Topic Exchange'e \"{queueName}\" isimli kuyruk key: '{Key}' ile birlikte bind edildi.");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static void Producer(string Key, string Message = null)
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.HostName = "localhost";
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        var body = Encoding.UTF8.GetBytes($"Merhaba! ben, key: '{Key}' için bir mesaj iletmeye geldim. Mesaj: {Message}");
                        channel.BasicPublish(exchange: "topic-exchange-test",
                                             routingKey: Key,
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine($"Mesaj key: '{Key}' ile birlikte \"topic-exchange-test\" isimli Topic Exchange'e gönderilmiştir");
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
