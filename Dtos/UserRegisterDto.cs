using System.ComponentModel.DataAnnotations;

namespace UrlService.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]{7,29}$", ErrorMessage = "can only contain alphabets and digits and underscore, and should begin with a word!")]
        public string UserName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Maximum 30 and Minimum 6 characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string EmialAddress { get; set; }
    }
}