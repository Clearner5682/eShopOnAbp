using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace EShopOnAbp.IdentityService.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestController> logger;
        private readonly IIdentityUserRepository userRepository;
        private readonly IIdentityRoleRepository roleRepository;

        public TestController(IConfiguration configuration, ILogger<TestController> logger, IIdentityUserRepository userRepository,IIdentityRoleRepository roleRepository)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        [HttpGet]
        [Route("postgre")]
        public async Task<IActionResult> Postgre()
        {
            var identityServiceConnectionString = this.configuration["ConnectionStrings:IdentityService"];
            this.logger.LogWarning($"IdentityServiceConnectionString:{identityServiceConnectionString}");
            var roleList = await this.roleRepository.GetListAsync();
            var userList = await this.userRepository.GetListAsync();


            return Ok(new { RoleList=roleList.Select(o=>new { RoleName=o.Name}),UserList=userList.Select(o=>new { UserName=o.UserName}) });
        }
    }
}
