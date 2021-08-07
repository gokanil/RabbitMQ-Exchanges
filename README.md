# RabbitMQ-Exchanges
Bu Uygulamayı rabbitmq'yu anlamak ve test etmek için yazdım.\
Uygulamada 3 proje var.bunların 2 tanesi Consumer, 1 taneside Producer'dir.

Producer:\
![Alt text](/../main/images/producer.png)

Producer İçin Örnek Komutlar:\
  DEFAULT => default salata hello world!\
  DIRECT => direct rambutan hello world!\
  FANOUT => fanout hello world!\
  TOPIC => topic sebze.patates.tohum hello world!\
  HEADER => header {key=patates lezzet=...} hello world!
  
  
komut testi: default salata hello world!
  
Consumer:\
![Alt text](/../main/images/test-hello-world.png)

</br>

Kaynaklar:\
https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html \
http://www.cengizhanunal.net/post/aspnet-core-rabbitmq-kullanimi \
https://www.gencayyildiz.com/blog/rabbitmq-fanout-exchange/ 
