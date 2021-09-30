using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UrlService.Dtos;
using UrlService.Enums;
using UrlService.Infrastructure;
using UrlService.Models;
using UrlService.Services;

namespace UrlService.Controllers
{
    [ApiController]
    [Route("analytics")]
    public class AnalyticsController : BaseController
    {
        private readonly UrlServiceDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IAnalyticsRecordSaver _analyticsRecordSaver;

        public AnalyticsController(UrlServiceDbContext context, IConfiguration configuration, ICacheService cacheService, IAnalyticsRecordSaver analyticsRecordSaver)
        {
            _dbContext = context;
            this._configuration = configuration;
            _cacheService = cacheService;
            _analyticsRecordSaver = analyticsRecordSaver;
        }

        [HttpGet("me/{id}")]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> AnalyseMyUrl([FromRoute] string id, [FromQuery] AnalyticsTypeEnum type)
        {
            if (id is null)
                return NotFound(new ServiceResult("id not found."));

            var userId = IUser.Id;
            var urlInfo = await _dbContext.UrlInfo.FindAsync(id);

            if (urlInfo is null)
                return NotFound(new ServiceResult("Url not found."));

            var timeRange = GetTimeRange(type);
            if (timeRange is null)
                return BadRequest(new ServiceResult("bad type!"));

            var result = _dbContext.AnalyticsData.Where(
                x => x.UserId.Equals(userId) && x.Path.Equals(urlInfo.ShortPath) && x.CreatedAt > timeRange.Start && x.CreatedAt < timeRange.End
                    );

            var groupByBrowser = result.GroupBy(x => x.Browser)
                .Select(x => new
                {
                    UrlId = id,
                    Path = urlInfo.ShortPath,
                    Browser = x.Key,
                    TotalSum = x.Sum(x => x.TotalCount),
                    MobileSum = x.Sum(x => x.IsMobileCount)
                });

            return Ok(ServiceResult.Create(groupByBrowser));
        }

        [HttpGet("all/{id}")]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> analyseUrl([FromRoute] string id, [FromQuery] AnalyticsTypeEnum type)
        {
            if (id is null)
                return NotFound(new ServiceResult("id not found."));

            var userId = IUser.Id;
            var urlInfo = await _dbContext.UrlInfo.FindAsync(id);

            if (urlInfo is null)
                return NotFound(new ServiceResult("Url not found."));

            var timeRange = GetTimeRange(type);
            if (timeRange is null)
                return BadRequest(new ServiceResult("bad type!"));

            var result = _dbContext.AnalyticsData.Where(
                x => x.MainUrl.Equals(urlInfo.MainUrl) && x.CreatedAt > timeRange.Start && x.CreatedAt < timeRange.End
                    );

            var groupByUser = result.AsEnumerable().GroupBy(x => x.UserId);
            var totalCount = groupByUser.Count();

            var groupByBrowser = result.AsEnumerable().GroupBy(x => x.Browser);

            var list = new List<BrowserResultsDto>();
            foreach (var k in groupByBrowser)
            {
                list.Add(new BrowserResultsDto
                {
                    Browser = k.Key,
                    UsersCount = k.GroupBy(x => x.UserId).Count()
                });
            }

            var mobileCount = 0;
            foreach (var u in groupByUser)
            {
                if (u.Any(x => x.IsMobileCount > 0))
                    mobileCount++;
            }

            var analytics = new
            {
                Url = urlInfo.MainUrl,
                TotalRedirectedUsers = totalCount,
                TotalRedirectedUsersOnMobile = mobileCount,
                BrowsersList = list
            };

            return Ok(ServiceResult.Create(analytics));
        }



        private TimeRangeDto GetTimeRange(AnalyticsTypeEnum e)
        {
            DateTime start, end, now;
            switch (e)
            {
                case AnalyticsTypeEnum.CurrentDay:
                    now = end = DateTime.Now;
                    start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case AnalyticsTypeEnum.Yesterday:
                    now = DateTime.Now;
                    var yesterday = now.AddDays(-1);
                    start = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
                    end = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                    break;
                case AnalyticsTypeEnum.Last7Days:
                    now = DateTime.Now;
                    yesterday = now.AddDays(-1);
                    var lastweek = now.AddDays(-7);
                    start = new DateTime(lastweek.Year, lastweek.Month, lastweek.Day, 0, 0, 0);
                    end = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                    break;
                case AnalyticsTypeEnum.Last30Days:
                    now = DateTime.Now;
                    yesterday = now.AddDays(-1);
                    var lastMonth = now.AddDays(-30);
                    start = new DateTime(lastMonth.Year, lastMonth.Month, lastMonth.Day, 0, 0, 0);
                    end = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                    break;
                default:
                    return null;
            }
            return new TimeRangeDto(start, end);
        }
    }
}