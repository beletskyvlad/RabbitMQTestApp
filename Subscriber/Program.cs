using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Press [enter] to start receiving messages.");
            Console.ReadLine();
            Console.Clear();
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "worker_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);

                var messagesReceived = 0;
                consumer.Received += (sender, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        var y = Console.CursorTop == 1 ? 2 : Console.CursorTop;
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine($"Messages received: {++messagesReceived}");
                        Console.SetCursorPosition(0, y);
                        Task.Run(() =>
                        {
                            Thread.Sleep(2000); //mocking some async action
                            Console.WriteLine($"Message: {message}. Thread #{Thread.CurrentThread.ManagedThreadId}.");
                        });
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                };
                channel.BasicConsume(queue: "worker_queue",
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}