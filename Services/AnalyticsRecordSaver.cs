using System;
using System.Threading.Tasks;
using UrlService.Dtos;
using UrlService.Infrastructure;
using UrlService.Models;

namespace UrlService.Services
{
    public interface IAnalyticsRecordSaver
    {
        Task WriteRecord(WriteRecordDto item);
    }

    public class AnalyticsRecordSaver : IAnalyticsRecordSaver
    {
        private readonly UrlServiceDbContext _dbContext;

        public AnalyticsRecordSaver(UrlServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task WriteRecord(WriteRecordDto item)
        {
            var record = new RedirectRecord
            {
                // Id = Guid.NewGuid().ToString(),
                IsMobile = item.IsMobile,
                Browser = item.Browser,
                UserId = item.UserId,
                MainUrl = item.MainUrl,
                Path = item.Path
            };
            await _dbContext.RedirectRecord.AddAsync(record);
            await _dbContext.SaveChangesAsync();
        }

    }
}