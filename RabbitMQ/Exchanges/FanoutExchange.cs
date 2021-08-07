using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Exchanges
{
    public static class FanoutExchange
    {
        public static void Consumer()
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.HostName = "localhost";
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "fanout-exchange-test",
                                                durable: false,
                                                autoDelete: true,
                                                type: ExchangeType.Fanout);

                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName,
                                          exchange: "fanout-exchange-test",
                                          routingKey: "");

                        channel.BasicQos(prefetchSize: 0,
                                         prefetchCount: 1,
                                         global: false);

                        var consumer = new EventingBasicConsumer(channel);
                        channel.BasicConsume(queueName,
                                             autoAck: false,
                                             consumer: consumer);

                        consumer.Received += (model, argument) =>
                        {
                            string message = Encoding.UTF8.GetString(argument.Body.ToArray());
                            Console.WriteLine();
                            Console.WriteLine(message);
                            channel.BasicAck(deliveryTag: argument.DeliveryTag, multiple: false);
                        };

                        Console.WriteLine($"'Consumer' başlatıldı. \"fanout-exchange-test\" isimli Fanout Exchange'e \"{queueName}\" isimli kuyruk bind edildi.");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static void Producer(string Message = null)
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

                        var body = Encoding.UTF8.GetBytes($"Merhaba! ben, toplu bir mesaj iletmeye geldim. Mesaj: {Message}");
                        channel.BasicPublish(exchange: "fanout-exchange-test",
                                             routingKey: "",
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine($"Mesaj \"fanout-exchange-test\" isimli Fanout Exchange'e gönderilmiştir");
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
