# RabbitMQ-Exchanges
Bu Uygulamayı rabbitmq'yu anlamak ve test etmek için yazdım.\
Uygulamada 3 proje var.bunların 2 tanesi Consumer, 1 taneside Producer'dir.\
RabbitMQ servisini çalıştırmak için uygulama konumundaki konsol penceresine 'docker-compose up -d' yazabilirsiniz.

Producer:\
![Alt text](/../main/images/producer.png)

<pre>
Producer İçin Örnek Komutlar:
  DEFAULT => default salata hello world!
  DIRECT => direct rambutan hello world!
  FANOUT => fanout hello world!
  TOPIC => topic sebze.patates.tohum hello world!
  HEADER => header {key=patates lezzet=...} hello world!
</pre>
  
Resimdeki örnek komut: default salata hello world!
  
Consumer:\
![Alt text](/../main/images/test-hello-world.png)

</br>
<pre>
Notlar:
 Exchange: Kuyruklara mesaj gönderirken, kuyrukları isteğe göre görevlendirir. Kuyruk ismi önemli değildir(default hariç).
  Default(Direct): Girilen 'routing key' ile aynı kuyruk ismine mesaj gönderir.
  Direct Exchange: Girilen 'routing key' ile eşleşen bütün kuyruklara mesaj gönderir.
  Fanout exchange: Key gerekmez. Bütün kuyruklara mesaj gönderir.
  Topic Exchange(.*#): '.', '*' ve '#' sembolleri ile kurallanmış uygun kuyruklara mesaj gönderir.
      '.' => örneğin 'sebze.patates.tohum', Routing Keyi tam olarak 'sebze.patates.tohum' ile eşleşen kuyruklara mesaj gönderir.
      '*' => örneğin 'sebze.*.tohum', Routing Keyi sebze ile başlayan ve tohum ile biten toplamda '3' gruptan oluşan kuyruklara mesaj gönderir.
      '#' => örneğin 'sebze.#.tohum', Routing Keyi sebze ile başlayan ve tohum ile biten bütün kuyruklara mesaj gönderir.
  Headers Exchange: örneğin '("sebze","patates") ("meyve","armut")', Routing Keyi tam olarak ("sebze","patates") ve ("meyve","armut") ile eşleşen kuyruklara mesaj gönderir.
</br>
 Metodlar:
  channel.ExchangeDeclare(exchange: "direct-exchange-test", durable: true, type: ExchangeType.Direct) => exchange yaratır.
     -exchange = "Exchange'nin ismi" 
     -durable  = "Verinin in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir. True ise kuyruk silinmesi veya brokerın tekrar başlatılması durumunda mesajlar durur."
     -type     = "Exchangenin türü(direct, fanout, headers, topic)"

  channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>(){{"x-max-priority", 2}}); => "Kuyruk  yaratır"(Queue)
     -queue      = "Kuyruğun ismi"
     -durable    = "Verinin in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir. True ise kuyruk silinmesi veya brokerın tekrar başlatılması durumunda mesajlar durur."
     -exclusive  = "True ise sadece bir connection'a bağlı kalır, connection ile bağlantısı koptuğunda kuyruğu siler."
     -autoDelete = "True ise kuyruk en az 1 mesaj aldıktan sonra eğer kuyruk tükenirse ve consumer dinlemeyi bırakırsa kuyruğu siler."
     -arguments  = "Kuyruk için ek parametreler."

  channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body); => "Kuyruğa mesaj gönderir"(Producer)
     -exchange        = "Exchange'nin ismi" 
     -routingKey      = "Girilen key’e göre ilgili kuyruğa mesajı gönderir."
     -basicProperties = 
     -body            = "Queue’ye gönderilecek mesaj. Mesaj byte[] tipinde gönderilir."
   
  channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer); => "Kuyruktaki veriyi okur"(Consumer)
     -queue    = "Kuyruğun ismi"
     -autoAck  = "True ise Consumer mesajı aldığı zaman otomatik olarak mesaj Queue’dan silinecektir."
     -cunsomer = 

  channel.BasicAck(deliveryTag: argument.DeliveryTag, multiple: false) => "kuyruktaki alır mesajı okur ve onaylar.(mesaj silinir)"
     -deliveryTag = "Kuyruktaki mesajın id sini belirtir. Id 1 den başlayarak otomatik artar.(64bit long)"
     -multiple    = "True ise deliveryTag 1 den şuanki gelen mesajın deliveryTag numarasına kadar mesajları onaylar. False ise sadece geleni onaylar"

  channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false) =>  Consumer'e gelen aynı anda mesaj sayısını sınırlar.
     -prefetchSize  = "prefetchCount sınırında mesaj alındıktan sonra, mesajların işlem sırasında diğer kuyruklarıda ram'a getirir."
     -prefetchCount = "Aynı anda kaç tane onaylanmamış mesaj alınabileceğini belirtir."
     -global        = "True ise channeller'in mesaj sınırına, False ise consumerler'in mesaj sınırına etki eder."
</pre>
</br>
<pre>
Arguments:
  x-queue-type => 
     -classic(default) => (rabbitmq tarafından desteği bırakılacak)
     -quorum =>
  x-expires => Kuyruğun otomatik olarak silinme isteği atıldıktan sonra, kuyruk belirtilen mili saniye türünden yaşam süresini boyunca hayatta kalır.
  x-message-ttl => Kuyruğun mili saniye türünden yaşam süresi belirler. Süre sonunda kuyruk silinir.
  x-max-length => Kuyruğun maksimum mesaj sayısını ayarlar.
  x-max-length-bytes => Kuyruğun maksimum byte boyutunu ayarlar.
  x-overflow => "Bir kuyruk max-length sınırını geçtiğinde, kuyruğun ilk mesajdan itibaren doğru uzunluğa girine kadar mesajları siler. Bunu owerflow ile değiştirebilirsin."
     -drop-head(default) => ↑
     -reject-publish => Kuyruk max-length sınırını geçtiğinde, nack ya atar.(tam anlayamadım.)
     -reject-publish-dlx => dead-letters rejected messages.
  x-max-priority => Kuyrukların önceliğini ayarlar.(default 0) 1 ile 255 (maksimum 10 öneriyor) arasında olabilir. Kuyruk yaratıldıktan sonra priority değiştirilemez.
  x-max-in-memory-length => Memory'deki kuyruğun maksimum mesaj sayısını ayarlar.
  x-max-in-memory-bytes => Memory'deki kuyruğun mesaj içeriğinin maksimum byte boyutunu ayarlar.


BasicProperties
  Expiration => x-message-ttl ↑↑
  Priority => x-max-priority ↑↑
  DeliveryMode => (1) Kalıcı olmayan, (2) kalıcı.
  Persistent => DeliveryMode'yi ayarlar.(true/false)
</pre>

Kaynaklar:\
https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html \
http://www.cengizhanunal.net/post/aspnet-core-rabbitmq-kullanimi \
https://www.gencayyildiz.com/blog/rabbitmq-fanout-exchange/ 
