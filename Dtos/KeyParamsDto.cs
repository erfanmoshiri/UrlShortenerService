namespace UrlService.Dtos
{
    public class KeyParamsDto
    {
        public KeyParamsDto(string path, string userId)
        {
            this.Path = path;
            this.UserId = userId;

        }
        public string Path { get; set; }
        public string UserId { get; set; }
    }
}