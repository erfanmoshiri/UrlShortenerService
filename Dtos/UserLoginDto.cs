namespace UrlService.Dtos
{
    public class UserLoginDto
    {
        // username or email
        public string Credential { get; set; }
        public string Password { get; set; }
    }


    public interface ILoginData
    {
        
    }
}