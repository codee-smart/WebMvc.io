namespace Clothings.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string JobDescription { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int salary { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}
