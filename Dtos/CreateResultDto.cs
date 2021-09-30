namespace UrlService.Dtos
{
    public class CreateResultDto
    {
        public CreateResultDto(string id)
        {
            this.Id = id;
        }
        public string Id { get; set; }
    }
}