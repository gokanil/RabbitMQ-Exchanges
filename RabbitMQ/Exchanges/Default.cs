using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Exchanges
{
    public static class Default
    {
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
                        channel.QueueDeclare(queue: Key,
                                             durable: false,
                                             exclusive: true,
                                             autoDelete: true,
                                             arguments: null);

                        channel.BasicQos(prefetchSize: 0,
                                         prefetchCount: 1,
                                         global: false);

                        var consumer = new EventingBasicConsumer(channel);
                        channel.BasicConsume(queue: Key,
                                             autoAck: false,
                                             consumer: consumer);

                        consumer.Received += (model, argument) =>
                        {
                            string message = Encoding.UTF8.GetString(argument.Body.ToArray());
                            Console.WriteLine();
                            Console.WriteLine(message);
                            channel.BasicAck(deliveryTag: argument.DeliveryTag, multiple: false);
                        };

                        Console.WriteLine($"'Consumer' başlatıldı. \"{Key}\" isimli kuyruk dinleniliyor.");
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

                        var body = Encoding.UTF8.GetBytes($"Merhaba! ben, kuyruk: '{Key}' için bir mesaj iletmeye geldim. Mesaj: {Message}");

                        channel.BasicPublish(exchange: "",
                                             routingKey: Key,
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine($"Mesaj (AMQP default) Exchange'e gönderilmiştir");
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
