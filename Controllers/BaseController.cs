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

        // string ClientId { get; }
        // string UniqeName { get; }
        // string FullName { get; }
        // IReadOnlyList<string> Scopes { get; }
        // IReadOnlyList<string> Roles { get; }
        // Task<string> EmailAsync();
        // Task<string> PhoneAsync();

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

            // this.principal = principal;
            //     UserData = new Dictionary<string,object>();
            //     if (principal != null)
            //         UserData = new Dictionary<string,object>()
            //         {
            //             {"sub",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("sub"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(IDentifiable.Empty)
            //                 .FirstOrDefault()},
            //             {"unique_name",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("unique_name"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"name",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("name"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"last_name",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("last_name"))
            //                 .Select(x => x.Value )
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"client_id",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("client_id"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"authorid",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("authorid"))
            //                 .Select(x => x.Value )
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"publisherid",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("publisherid"))
            //                 .Select(x => x.Value )
            //                 .DefaultIfEmpty(string.Empty)
            //                 .FirstOrDefault()},
            //             {"scope",principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("scope"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(string.Empty)
            //                 .ToList()},
            //             {"role", principal.Claims
            //                 .Where(x => x.Type.ToLower().Equals("role"))
            //                 .Select(x => x.Value)
            //                 .DefaultIfEmpty(string.Empty)
            //                 .ToList()},
            //         };
            // }
            // public static IUserHelper CreateUser(
            //     string id,string fn,string ln,string un,string ci,string ai,string pi,IReadOnlyList<string> scopes ,IReadOnlyList<string> role
            // )
            // {
            //     var dic = new Dictionary<string,object>();
            //     dic["sub"] = id;
            //     dic["unique_name"] = un;
            //     dic["name"] = fn;
            //     dic["last_name"] = ln;
            //     dic["client_id"] = ci;
            //     dic["authorid"] = ai;
            //     dic["publisherid"] = pi;
            //     dic["scope"] = scopes;
            //     dic["role"] = role;
            //     var uh = new UserHelper(null);
            //     uh.UserData = dic;
            //     return uh;
            // }
            // public IDentifiable Id => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("sub"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(IDentifiable.Empty)
            //     .FirstOrDefault();

            // public string UniqeName => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("unique_name"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();
            // public string FirstName => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("name"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();

            // public string LastName => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("last_name"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();
            // public string FullName => $"{FirstName} {LastName}";

            // public string ClientId => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("client_id"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();

            // public string AuthorId => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("authorid"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();

            // public string PublisherId => this.UserData
            //     .Where(x => x.Key.ToLower().Equals("publisherid"))
            //     .Select(x => x.Value + "")
            //     .DefaultIfEmpty(string.Empty)
            //     .FirstOrDefault();

            // public IReadOnlyList<string> Scopes =>
            //         this.UserData
            //                 .Where(x => x.Key.ToLower().Equals("scope"))
            //                 .Select(x => x.Value + "")
            //                 .DefaultIfEmpty(string.Empty)
            //                 .ToList();
            // public IReadOnlyList<string> Roles =>
            // this.UserData
            //                 .Where(x => x.Key.ToLower().Equals("role"))
            //                 .Select(x => x.Value + "")
            //                 .DefaultIfEmpty(string.Empty)
            //                 .ToList();
            // public Task<string> EmailAsync()
            // {
            //     return Task.FromResult("");
            // }

            // public async Task<T> GetGustUserAsync<T>(string userId)
            // {
            //     return await Task.FromResult<T>(default(T));
            // }

            // public Task<string> PhoneAsync()
            // {
            //     return Task.FromResult("");
            // }
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