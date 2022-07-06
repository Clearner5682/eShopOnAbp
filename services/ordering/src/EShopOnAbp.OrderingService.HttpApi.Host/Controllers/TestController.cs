using EShopOnAbp.OrderingService.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EShopOnAbp.OrderingService.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestController> logger;
        private readonly IOrderRepository orderRepository;

        public TestController(IConfiguration configuration, ILogger<TestController> logger, IOrderRepository orderRepository)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.orderRepository = orderRepository;
        }

        [HttpGet]
        [Route("postgre")]
        public async Task<IActionResult> Postgre()
        {
            var orderingServiceConnectionString = this.configuration["ConnectionStrings:OrderingService"];
            this.logger.LogWarning($"OrderingServiceConnectionString:{orderingServiceConnectionString}");
            var orderList = await this.orderRepository.GetListAsync();


            return Ok(new { OrderList=orderList.Select(o=>new { id=o.Id,orderno=o.OrderNo,orderdate=o.OrderDate}) });
        }
    }
}
