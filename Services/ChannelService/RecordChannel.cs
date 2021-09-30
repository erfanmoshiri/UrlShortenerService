using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrlService.Dtos;
using UrlService.Infrastructure;
using UrlService.Models;

namespace Book.Api.Channels
{
    public class RecordChannel : IRecordChannel
    {
        private Channel<WriteRecordDto> channel { get; set; }
        // private readonly UrlServiceDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        public RecordChannel(IServiceProvider serviceProvider)
        {
            channel = Channel.CreateUnbounded<WriteRecordDto>();
            ReadAsync();
            _serviceProvider = serviceProvider;
        }

        public async Task WriteAsync(WriteRecordDto dto)
        {
            await channel.Writer.WriteAsync(dto);
        }



        public async ValueTask ReadAsync()
        {
            while (await channel.Reader.WaitToReadAsync())
            {
                if (channel.Reader.TryRead(out var item))
                {
                    try
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
                        using (IServiceScope scope = _serviceProvider.CreateScope())
                        {
                            UrlServiceDbContext db =
                                scope.ServiceProvider.GetRequiredService<UrlServiceDbContext>();

                            // await scopedProcessingService.DoWorkAsync(stoppingToken);
                            await db.RedirectRecord.AddAsync(record);
                            await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        ///Exceptions should be passed to keep the loop alive
                        System.Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}