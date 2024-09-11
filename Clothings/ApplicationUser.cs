using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clothings
{
    public class ApplicationUser :IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? State { get; set; }
    }
}
