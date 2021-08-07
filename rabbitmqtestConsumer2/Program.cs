using RabbitMQ.Exchanges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rabbitmqtestConsumer2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uyarı: Key ve Headers değerleri girerken Türkçe karakter kullanırsanız daha sonrasında direkt olarak işlem yapamazsınız.
            Parallel.Invoke(
                () => Default.Consumer("salata"),
                () => DirectExchange.Consumer("patates"),
                () => FanoutExchange.Consumer(),
                () => TopicExchange.Consumer("sebze.patates.*"),
                () => HeaderExchange.Consumer(new Dictionary<string, object>() { ["key"] = "kabak" }));
        }
    }
}
