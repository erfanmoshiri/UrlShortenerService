using UrlService.Models.BaseModels;

namespace UrlService.Models
{
    public class RedirectRecord : BaseModel
    {
        public bool IsMobile { get; set; }
        public string Browser { get; set; }
        public string UserId { get; set; }
        public string MainUrl { get; set; }
        public string Path { get; set; }
    }
}