using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UrlService.Dtos;
using UrlService.Infrastructure;
using UrlService.Models;

namespace UrlService.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UrlServiceDbContext _context;

        private readonly IConfiguration _configuration;
        private readonly ITokenFactory _tokenFactory;
        public UserManager<User> UserManager { get; }
        public SignInManager<User> SignInManager { get; }

        public UserController(UrlServiceDbContext context, UserManager<User> UserManager, SignInManager<User> SignInManager, IConfiguration configuration, ITokenFactory tokenFactory)
        {
            _context = context;
            this.UserManager = UserManager;
            this.SignInManager = SignInManager;
            this._configuration = configuration;
            _tokenFactory = tokenFactory;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult>> Register(UserRegisterDto user)       //register
        {

            if (ModelState.IsValid)
            {
                User user1 = new User { UserName = user.UserName, Name = user.Name, Email = user.EmialAddress };
                var result = await UserManager.CreateAsync(user1, user.Password);
                if (!result.Succeeded)
                {
                    var ErrorMessage = "";
                    foreach (var error in result.Errors)
                    {
                        ErrorMessage += error.Description;
                    }
                    return BadRequest(new ServiceResult(ErrorMessage));
                }
                else
                {
                    return Ok(ServiceResult.Create("Registered! now login please."));
                }

            }
            else
            {
                var ErrorMessage = "";
                foreach (var item in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ErrorMessage += item.ErrorMessage.ToString() + "\n";
                }
                return BadRequest(new ServiceResult(ErrorMessage));
            }

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult>> LoginUserAsync(UserLoginDto model)
        {
            var user0 = await UserManager.FindByNameAsync(model.Credential);
            var user1 = await UserManager.FindByEmailAsync(model.Credential);

            var user = user0;
            if(user0 is null)
                user = user1;

            if(user == null)
            {
                return NotFound(new ServiceResult("user not found"));
            }

            var result = await UserManager.CheckPasswordAsync(user, model.Password);

            if(!result)
                return NotFound(new ServiceResult("Incorrect password"));

            var token = _tokenFactory.Generate(user);

            return Ok(ServiceResult<ITokenResult>.Create(token));
        }

    }
}