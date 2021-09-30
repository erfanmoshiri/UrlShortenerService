namespace UrlService.Dtos
{
    public class UrlCreateDto
    {

        public UrlCreateDto(string mainUrl, string suggestedPath, string description)
        {
            this.MainUrl = mainUrl;
            this.SuggestedPath = suggestedPath;
            this.Description = description;

        }
        public string MainUrl { get; set; }
        public string SuggestedPath { get; set; }
        public string Description { get; set; }
    }
}