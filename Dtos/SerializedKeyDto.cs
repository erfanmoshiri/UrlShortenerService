namespace UrlService.Dtos
{
    public class SerializedKeyDto
    {
        public SerializedKeyDto(string userId, string path, string browser)
        {
            this.userId = userId;
            this.path = path;
            this.browser = browser;

        }
        public string userId { get; set; }
        public string path { get; set; }
        public string browser { get; set; }
    }
}