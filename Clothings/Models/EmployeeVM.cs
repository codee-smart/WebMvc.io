using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Clothings.Models
{
    public class EmployeeVM
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string JobDescription { get; set; } = "";
        public IEnumerable<SelectListItem> EmpTypesList { get; set; } = new List<SelectListItem>();


        [Required]
        public int PhoneNumber { get; set; } 

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        public int salary { get; set; }

        public DateTime JoiningDate { get; set; }
    }
}
