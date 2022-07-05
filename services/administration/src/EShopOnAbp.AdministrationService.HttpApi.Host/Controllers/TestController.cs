using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.PermissionManagement;

namespace EShopOnAbp.AdministrationService.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestController> logger;
        private readonly IPermissionGrantRepository repository;

        public TestController(IConfiguration configuration,ILogger<TestController> logger, IPermissionGrantRepository repository)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet]
        [Route("postgre")]
        public async Task<IActionResult> Postgre()
        {
            var administrationConnectionString = this.configuration["ConnectionStrings:AdministrationService"];
            this.logger.LogInformation($"AdministrationConnectionString:{administrationConnectionString}");
            var permissionGrantList = await this.repository.GetListAsync();

            return Ok(new { PermissionGrantList = permissionGrantList.Select(o =>new {id=o.Id,name=o.Name,providername=o.ProviderName,providerkey=o.ProviderKey })});
        }
    }
}
