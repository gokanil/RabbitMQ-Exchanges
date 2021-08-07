using RabbitMQ.Exchanges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rabbitmqtestConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uyarı: Key ve Headers değerleri girerken Türkçe karakter kullanırsanız daha sonrasında direkt olarak işlem yapamazsınız.
            Parallel.Invoke(
                () => Default.Consumer("biber"),
                () => DirectExchange.Consumer("rambutanş"),
                () => FanoutExchange.Consumer(),
                () => TopicExchange.Consumer("sebze.#.tohum"),
                () => HeaderExchange.Consumer(new Dictionary<string, object>() { ["key"] = "patates", ["lezzet"] = "..." }));
        }
    }
}
