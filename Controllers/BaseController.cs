using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UrlService.Controllers
{
    public interface IUserHelper
    {
        string Id { get; }
        string Name { get; }
        string UserName { get; }
    }

    public class UserHelper : IUserHelper
    {

        public IDictionary<string, object> UserData { get; private set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public UserHelper(System.Security.Claims.ClaimsPrincipal principal)
        {
            this.UserName = principal.Claims
                            .Where(x => x.Type.ToLower().Equals("username"))
                            .Select(x => x.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault();

            this.Id = principal.Claims
                            .Where(x => x.Type.ToLower().Equals("id"))
                            .Select(x => x.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault();

            this.Name = principal.Claims
                            .Where(x => x.Type.ToLower().Equals("name"))
                            .Select(x => x.Value)
                            .DefaultIfEmpty(string.Empty)
                            .FirstOrDefault();

        }
    }

    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger logger;

        public BaseController()
        {

        }

        public BaseController(ILogger logger)
        {
            this.logger = logger;
        }

        public IUserHelper IUser => new UserHelper(this.User);

    }
}