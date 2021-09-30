using UrlService.Models.BaseModels;

namespace UrlService.Models
{
    public class AnalyticsData : BaseModel
    {
        public string UserId { get; set; }
        public string Path { get; set; }
        public string MainUrl { get; set; }
        public string Browser { get; set; }
        public int IsMobileCount { get; set; }
        public int TotalCount { get; set; }


    }
}