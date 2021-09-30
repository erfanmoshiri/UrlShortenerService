using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UrlService.Dtos;
using UrlService.Infrastructure;
using UrlService.Models;

namespace UrlService.Services
{
    public class BackgroundService
    {
        private readonly UrlServiceDbContext _dbContext;

        public BackgroundService(UrlServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CollectAndClean()
        {
            var now = DateTime.Now;
            var expiredData = _dbContext.RedirectRecord.Where(x => x.CreatedAt <= now).ToList();

            var cleanedData = expiredData
                .GroupBy(x => new { x.UserId, x.Path, x.Browser })
                .Select(
                    x => new AnalyticsData
                    {
                        UserId = x.Key.UserId,
                        Path = x.Key.Path,
                        Browser = x.Key.Browser,
                        MainUrl = x.First().MainUrl,
                        IsMobileCount = x.Count(y => y.IsMobile),
                        TotalCount = x.Count()
                    }
                );
            // var SerializedKeysList = cleanedData.Select(x => x.SerializedKey);

            // var matchedRecords = _dbContext.AnalyticsData.Where(x => SerializedKeysList.Contains(x.SerializedKey));
            // foreach (var m in matchedRecords)
            // {
            //     var cleaned = cleanedData.FirstOrDefault(x => x.SerializedKey.Equals(m.SerializedKey));
            //     if (cleaned != null)
            //     {
            //         m.TotalCount += cleaned.TotalCount;
            //         m.IsMobileCount += cleaned.IsMobileCount;
            //     }
            // }
            var addTask = _dbContext.AnalyticsData.AddRangeAsync(cleanedData);
            _dbContext.RedirectRecord.RemoveRange(expiredData);
            Task.WaitAll(addTask);
            _dbContext.SaveChanges();
        }

        public async Task FlushOldData()
        {
            var now = DateTime.Now;
            var _30DaysBefore = now.AddDays(-30);
            var dataToClean = _dbContext.AnalyticsData.Where(x => x.CreatedAt < _30DaysBefore);
            _dbContext.AnalyticsData.RemoveRange(dataToClean);
            await _dbContext.SaveChangesAsync();
        }

        // public string KeysSerializer(SerializedKeyDto item) => JsonConvert.SerializeObject(item);
    }


}