using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Exchanges
{
    public static class HeaderExchange
    {
        public static void Consumer(Dictionary<string, object> Headers)
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.HostName = "localhost";
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "header-exchange-test",
                                                durable: false,
                                                autoDelete: true,
                                                type: ExchangeType.Headers);

                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName,
                                          exchange: "header-exchange-test",
                                          routingKey: "",
                                          arguments: Headers);

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

                        Console.WriteLine($"'Consumer' başlatıldı. \"header-exchange-test\" isimli Header Exchange'e \"{queueName}\" isimli kuyruk key: '{JsonSerializer.Serialize(Headers)}' ile birlikte bind edildi.");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static void Producer(Dictionary<string, object> Headers, string Message = null)
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
                        properties.Headers = Headers;

                        var body = Encoding.UTF8.GetBytes($"Merhaba! ben, key: '{JsonSerializer.Serialize(Headers)}' için bir mesaj iletmeye geldim. Mesaj: {Message}");

                        channel.BasicPublish(exchange: "header-exchange-test",
                                             routingKey: "",
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine($"Mesaj key: '{JsonSerializer.Serialize(Headers)}' ile birlikte \"header-exchange-test\" isimli Header Exchange'e gönderilmiştir");
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
