using EShopOnAbp.CatalogService.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EShopOnAbp.CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestController> logger;
        private readonly IRepository<Product> repository;

        public TestController(IConfiguration configuration, ILogger<TestController> logger, IRepository<Product, Guid> repository)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet]
        [Route("mongodb")]
        public async Task<IActionResult> MongoDb()
        {
            var mongodbConnectionString = this.configuration["ConnectionStrings:CatalogService"];
            this.logger.LogWarning($"MongodbConnectionString:{mongodbConnectionString}");
            var productList = await this.repository.GetListAsync();

            return Ok(new { ProductList=productList });
        }
    }
}
