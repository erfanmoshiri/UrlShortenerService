using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Book.Api.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UrlService.Dtos;
using UrlService.Infrastructure;
using UrlService.Models;
using UrlService.Services;

namespace UrlService.Controllers
{
    [ApiController]
    [Route("url")]
    public class UrlController : BaseController
    {
        private readonly UrlServiceDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IRecordChannel _recordChannel;

        public UrlController(UrlServiceDbContext context, IConfiguration configuration, ICacheService cacheService, IRecordChannel recordChannel)
        {
            _dbContext = context;
            this._configuration = configuration;
            _cacheService = cacheService;
            this._recordChannel = recordChannel;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> Create([FromBody] UrlCreateDto item)
        {
            if (string.IsNullOrEmpty(item?.MainUrl ?? null))
                return BadRequest(new ServiceResult("you shoud define main Url."));

            var userId = IUser.Id;
            var userName = IUser.UserName;

            if (string.IsNullOrEmpty(item.SuggestedPath))
            {
                var randomPath = RandomGenerator();
                var urlInfo = new UrlInfo(item.Description, item.MainUrl, randomPath, userId);
                await _dbContext.UrlInfo.AddAsync(urlInfo);
                await _dbContext.SaveChangesAsync();
                return Ok(ServiceResult.Create(new CreateResultDto(urlInfo.Id)));

            }
            else
            {
                var exists = _dbContext.UrlInfo.FirstOrDefault(x => !x.IsDeleted && x.UserId.Equals(userId) && x.ShortPath.Equals(item.SuggestedPath)) != null;
                if (exists)
                {
                    return BadRequest(new ServiceResult("your suggested path is already linked to another url of yours. please choose another one"));
                }
                var urlInfo = new UrlInfo(item.Description, item.MainUrl, item.SuggestedPath, userId);
                await _dbContext.UrlInfo.AddAsync(urlInfo);
                await _dbContext.SaveChangesAsync();
                return Ok(ServiceResult.Create(new CreateResultDto(urlInfo.Id)));
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ServiceResult("no id given"));

            var userId = IUser.Id;

            var oldUrl = _dbContext.UrlInfo.FirstOrDefault(x => x.Id.Equals(id));
            if (oldUrl is null || !oldUrl.UserId.Equals(userId))
                return NotFound(new ServiceResult("no Url info was found"));

            oldUrl.SetDeleted();
            _dbContext.UrlInfo.Update(oldUrl);
            await _dbContext.SaveChangesAsync();
            return Ok(ServiceResult.Create("Successful"));
        }


        [HttpGet("all")]
        [Authorize]
        public ActionResult<ServiceResult> GetAll([FromQuery] int pgNumber, [FromQuery] int pgSize)
        {
            if (pgNumber < 0 || pgSize <= 0)
                return BadRequest(new ServiceResult("define a valid page number and page size please"));

            var userId = IUser.Id;
            var allUrls = _dbContext.UrlInfo
                        .Where(x => x.IsDeleted && x.UserId.Equals(userId))
                        .Skip(pgNumber * pgSize)
                        .Take(pgSize)
                        .Select(x => new UrlResultDto(
                            x.Id, x.MainUrl, x.ShortPath, x.Description, UrlGenerator(x.ShortPath)
                        ))
                        .ToList();

            return Ok(ServiceResult.Create(allUrls));
        }

        [HttpGet("r/{path}")]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> RedirectTo([FromRoute] string path)
        {
            var userId = IUser.Id;
            if (string.IsNullOrEmpty(path))
                return BadRequest(new ServiceResult("incorrect url"));
            string mainUrl;

            var keyInCache = JsonConvert.SerializeObject(new KeyParamsDto(path, userId));
            var resultInCache = await _cacheService.GetAsynce(keyInCache);

            if (resultInCache is null)
            {
                var resultInDb = _dbContext.UrlInfo.FirstOrDefault(x => x.ShortPath.Equals(path) && x.UserId.Equals(userId));
                if (resultInDb is null)
                    return NotFound(new ServiceResult("Url not found."));
                mainUrl = resultInDb.MainUrl;

                var valueInCache = JsonConvert.SerializeObject(new ValueParamsDto(resultInDb.MainUrl));
                _cacheService.WriteAsynce(keyInCache, valueInCache);
            }
            else
            {
                var dto = JsonConvert.DeserializeObject<ValueParamsDto>(resultInCache);
                mainUrl = dto?.MainUrl;
            }

            var browser = Request.Headers["User-Agent"].ToString();
            var isMobile = browser.ToLower().Contains("mobi");

            var record = new WriteRecordDto
            {
                IsMobile = isMobile,
                Browser = browser,
                UserId = userId,
                MainUrl = mainUrl,
                Path = path
            };
            await _recordChannel.WriteAsync(record);
            return Redirect(mainUrl);
        }




        public static string UrlGenerator(string path) => $"https://localhost:5001/r/{path}";
        public static string RandomGenerator()
        {
            Random rnd = new Random();
            string randomPath = "";
            char c;
            for (int i = 0; i < 10; i++)
            {
                var rn = rnd.Next(2);
                if (rn == 1)
                    c = (char)rnd.Next(65, 91);
                else
                    c = (char)rnd.Next(97, 123);
                randomPath += c;
            }
            return randomPath;
        }
    }
}