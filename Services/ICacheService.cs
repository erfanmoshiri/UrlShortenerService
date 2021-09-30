using System.Threading.Tasks;

namespace UrlService.Services
{
    public interface ICacheService
    {
         Task<string> GetAsynce(string key);
         Task WriteAsynce(string key, string value);
    }
}