using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.RabbitMQ;
using Volo.Abp.EventBus.RabbitMq;
using RabbitMQ.Client;
using System.Text;
using System;
using Volo.Abp.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace EShopOnAbp.AuthServer
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<TestController> logger;

        public TestController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<TestController> logger
            )
        {
            this.configuration = configuration;
            this.env = env;
            this.logger = logger;
            var envName = env.EnvironmentName;
            var selfUrl = configuration["App:SelfUrl"];
            var authServer = configuration["AuthServer:Authority"];
            logger.LogWarning($"env:{envName},self_url:{selfUrl},auth_server:{authServer}");
        }

        [HttpGet]
        [Route("rabbitmq")]
        public IActionResult RabbitMq(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                return Ok(new { Message="value is null"});
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
                channel.QueueDeclare(queue:queueName,durable:true,exclusive:false, autoDelete:false,null);
                byte[] body = Encoding.UTF8.GetBytes(value);

                channel.BasicPublish("", queueName, null, body);
            }
            catch(Exception ex)
            {
                return Ok(new { Message = ex.Message });
            }

            return Ok(new { Value=value});
        }

        [HttpGet]
        [Route("redis")]
        public IActionResult Redis(string key,string value, string type = "get")
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);

            this.logger.LogWarning("redis:" + configuration["Redis:Configuration"]);

            if (key.IsNullOrEmpty()||value.IsNullOrEmpty())
            {
                return Ok(new { Message="key or value is null"});
            }
            if(type == "get")
            {
                var redisValue = redis.GetDatabase().StringGet(key);
                if (redisValue.HasValue)
                {
                    return Ok(new { Type = "Get", Key = key, Value = redisValue.ToString() });
                }

                return Ok(new { Type = "Get", Key = key, Value ="" });
            }
            else
            {
                redis.GetDatabase().StringSet(key, value);

                return Ok(new { Type = "Set", Key = key, Value = value });
            }
        }

        [HttpGet]
        [Route("postgre")]
        public IActionResult Postgre()
        {


            return Ok();
        }
    }
}
