using System.ComponentModel.DataAnnotations;

namespace Clothings.Models
{
    public class RegisterVM
    {
        public ApplicationUser applicationUser { get; set; }
        public string StatusMessage { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Didn't Match")]
        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
