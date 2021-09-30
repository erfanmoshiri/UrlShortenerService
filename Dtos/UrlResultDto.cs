namespace UrlService.Dtos
{
    public class UrlResultDto : UrlCreateDto
    {
        public UrlResultDto(string id, string mainUrl, string suggestedPath, string description, string newUrl)
            : base(mainUrl, suggestedPath, description)
        {
            this.Id = id;
            NewUrl = newUrl;
        }
        public string Id { get; set; }
        public string NewUrl { get; set; }
    }
}