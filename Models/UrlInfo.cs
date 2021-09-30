using UrlService.Models.BaseModels;

namespace UrlService.Models
{
    public class UrlInfo : BaseModel
    {
        public UrlInfo(string description, string mainUrl, string shortPath, string userId)
        {
            this.Description = description;
            this.MainUrl = mainUrl;
            this.ShortPath = shortPath;
            this.UserId = userId;

        }
        public string Description { get; set; }
        public string MainUrl { get; set; }
        public string ShortPath { get; set; }
        public string UserId { get; set; }


        public bool SetDeleted()
        {
            this.IsDeleted = true;
            return this.IsDeleted;
        }
    }
}