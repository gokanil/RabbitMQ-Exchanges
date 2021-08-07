using RabbitMQ.Exchanges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rabbitmqtestProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#'Producer' başlatılmadan önce 'Consumer' çalıştırılmalı. Bunun sebebi Exchange kullanan Producer'ler kuyrukları rastgele bir isimle oluşturuyor. Eğer kuyruk oluşmadan Producer mesaj göndermeye çalışırsa ilgili Exchanged'e bind edilmiş bir kuyruk bulamaz.");
            Console.WriteLine();

            //Örnek Komutlar
            /*
             * DEFAULT => default salata hello world!
             * DIRECT => direct rambutan hello world!
             * FANOUT => fanout hello world!
             * TOPIC => topic sebze.patates.tohum hello world!
             * HEADER => header {key=patates lezzet=...} hello world!
             */
            Console.WriteLine("DEFAULT [QUEUENAME] [MESSAGE]");
            Console.WriteLine("DIRECT [KEY] [MESSAGE]");
            Console.WriteLine("FANOUT [MESSAGE]");
            Console.WriteLine("TOPIC [KEY] [MESSAGE]");
            Console.WriteLine("HEADER {[KEY]=[VALUE] [KEY]=[VALUE]..} [MESSAGE]");
            Console.WriteLine();

            while (true)
            {
                Console.Write("=> ");

                var exchangeParts = Console.ReadLine().Split(' ', 3);

                var exchange = exchangeParts.ElementAtOrDefault(0).ToUpperInvariant();
                var key = exchangeParts.ElementAtOrDefault(1);
                var message = exchangeParts.ElementAtOrDefault(2);
                var skipCommand = string.Join(" ", exchangeParts.Skip(1));

                if (exchange == "DEFAULT" || exchange == "D")
                    Default.Producer(key, message);
                else if (exchange == "DIRECT" || exchange == "DI")
                    DirectExchange.Producer(key, message);
                else if (exchange == "FANOUT" || exchange == "F")
                    FanoutExchange.Producer(skipCommand);
                else if (exchange == "TOPIC" || exchange == "T")
                    TopicExchange.Producer(key, message);
                else if (exchange == "HEADER" || exchange == "H")
                    HeaderExchange.Producer(SplitHeaderExchange(skipCommand), message);

                Console.WriteLine();
            }
        }

        static Dictionary<string, object> SplitHeaderExchange(string header)
        {
            var exchangeParts = header.Split(new char[] { '{', '}' }, 3);

            string headerLine = exchangeParts.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(headerLine))
                return null;

            var result = headerLine
                 .Split(' ')
                 .Select(x => x.Split('='))
                 .ToDictionary(x => x.ElementAtOrDefault(0), x => (object)x.ElementAtOrDefault(1));

            return result;
        }
    }
}
