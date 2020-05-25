using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static readonly Random rand = new Random();

        static void Main(string[] args)
        {
            Console.Write("Press [enter] to continue...");
            Console.ReadLine();
            var messagesSentCount = 0;
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes("test");

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                Console.WriteLine("Press [escape] to stop publishing messages.");
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        channel.BasicPublish(exchange: "",
                         routingKey: "task_queue",
                         basicProperties: properties,
                         body: body);
                        Console.WriteLine($"Messages sent: {++messagesSentCount}");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Thread.Sleep(300);
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }

            Console.ReadLine();
        }
    }
}
