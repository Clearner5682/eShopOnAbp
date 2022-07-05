using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace EShopOnAbp.BasketService.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestController> logger;

        public TestController(IConfiguration configuration,ILogger<TestController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [HttpGet]
        [Route("rabbitmq")]
        public IActionResult RabbitMq(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                return Ok(new { Message = "value is null" });
            }
            string queueName = "TestQueue";
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = configuration["RabbitMQ:Connections:Default:HostName"];

            this.logger.LogWarning("rabbitmq:" + configuration["RabbitMQ:Connections:Default:HostName"]);

            IConnection connection;
            try
            {
                connection = connectionFactory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, null);
                byte[] body = Encoding.UTF8.GetBytes(value);

                channel.BasicPublish("", queueName, null, body);
            }
            catch (Exception ex)
            {
                return Ok(new { Message = ex.Message });
            }

            return Ok(new { Value = value });
        }
    }
}
