using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Publisher
{
    class Program
    {
        private static readonly Random Rand = new Random();

        static void Main(string[] args)
        {
            Console.Write("Press [enter] to start publishing messages.");
            Console.ReadLine();
            var messagesSentCount = 0;
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine("Press [escape] to stop publishing messages.");
                channel.QueueDeclare(queue: "worker_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                const string body = "test";
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        var randomPayload = Rand.Next(10_000).ToString();
                        var bodyBytes = Encoding.UTF8.GetBytes(body + randomPayload);
                        channel.BasicPublish(exchange: "",
                                             routingKey: "worker_queue",
                                             basicProperties: properties,
                                             body: bodyBytes);
                        Console.WriteLine($"Messages sent: {++messagesSentCount}");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Thread.Sleep(Rand.Next(1000)); //publish the next event with some delay
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
