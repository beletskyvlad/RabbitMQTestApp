using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Press [enter] to continue...");
            Console.ReadLine();
            Console.Clear();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 6, global: false);

                var consumer = new EventingBasicConsumer(channel);

                var messagesReceived = 0;
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    // Note: it is possible to access the channel via
                    //       ((EventingBasicConsumer)sender).Model here
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    Console.WriteLine($"Messages received: {++messagesReceived}");
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Thread.Sleep(500);
                };
                channel.BasicConsume(queue: "task_queue",
                                     autoAck: false,
                                     consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}