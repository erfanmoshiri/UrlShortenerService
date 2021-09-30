using System.Threading.Tasks;
using UrlService.Dtos;
using UrlService.Infrastructure;

namespace Book.Api.Channels
{
    public interface IRecordChannel
    {
        Task WriteAsync(WriteRecordDto dto);
        ValueTask ReadAsync();
    }
}