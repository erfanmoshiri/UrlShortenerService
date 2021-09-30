namespace UrlService.Dtos
{
    public class ValueParamsDto
    {
        public ValueParamsDto(string mainUrl)
        {
            this.MainUrl = mainUrl;

        }
        public string MainUrl { get; set; }
    }
}