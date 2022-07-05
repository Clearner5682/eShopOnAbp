using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EShopOnAbp.BasketService
{
    public class TestHostedService : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestHostedService> logger;

        public TestHostedService(IConfiguration configuration, ILogger<TestHostedService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "TestQueue";
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = this.configuration["RabbitMQ:Connections:Default:HostName"];
            IConnection connection;
            try
            {
                connection = connectionFactory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, arg) =>
                {
                    byte[] body = arg.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);
                    this.logger.LogWarning("Consume:" + message);
                    channel.BasicAck(deliveryTag: arg.DeliveryTag, multiple: false);
                    Thread.Sleep(500);
                };
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error:" + ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
