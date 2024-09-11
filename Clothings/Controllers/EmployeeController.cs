using Clothings.Data;
using Clothings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clothings.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var employee = context.Employees
                    .OrderByDescending(p => p.Id)
                    .ToList();

            return View(employee);

        }
        private IEnumerable<SelectListItem> GetTypesList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Chef", Value = "1" },
                new SelectListItem { Text = "Waiter", Value = "2" },
                new SelectListItem { Text = "Security Staff", Value = "3" },
                new SelectListItem { Text = "Cleaning Staff", Value = "4" },
                new SelectListItem { Text = "Others", Value = "5" }
            };
        }

        public IActionResult Create()
        {
            var employeeVM = new EmployeeVM
            {
                
                EmpTypesList = GetTypesList()
            };

            return View(employeeVM);
        }

        [HttpPost]
        public IActionResult Create(EmployeeVM employeeVM)
        {
            if (!ModelState.IsValid)
            {
                employeeVM.EmpTypesList = GetTypesList();
                return View(employeeVM);
            }

            string typeName = GetTypeName(employeeVM.JobDescription);
            // Save the new product into the database
            Employee employee = new Employee()
            {
                Name = employeeVM.Name,
                JobDescription = typeName,
                PhoneNumber = employeeVM.PhoneNumber,
                EmailAddress = employeeVM.EmailAddress,
                salary = employeeVM.salary,
                JoiningDate = DateTime.Now,
            };

            context.Employees.Add(employee);
            context.SaveChanges();
            return RedirectToAction("Index", "Employee");
        }

        private string GetTypeName(string JobDescription)
        {
            return JobDescription switch
            {
                "1" => "Chef",
                "2" => "Waiter",
                "3" => "Security Staff",
                "4" => "Cleaning Staff",
                "5" => "Others",
                _ => "Unknown" // Default case for unmatched values
            };
        }
        public IActionResult Edit(int id)
        {
            var employee = context.Employees.FirstOrDefault(p => p.Id == id);

            if (employee == null)
            {
                return RedirectToAction("Index", "Employee");
            }

            string typeName = GetTypeName(employee.JobDescription);
            var employeeVM = new EmployeeVM()
            {
                Name = employee.Name,
                JobDescription = employee.JobDescription, // Use JobDescription directly
                PhoneNumber = employee.PhoneNumber,
                EmailAddress = employee.EmailAddress,
                salary = employee.salary,
                EmpTypesList = GetTypesList() // Set the dropdown list
            };

            ViewData["EmployeeId"] = employee.Id;
            ViewData["JoiningDate"] = employee.JoiningDate.ToString("MM/dd/yyyy");

            return View(employeeVM);
        }



        [HttpPost]
        public IActionResult Edit(int id, EmployeeVM employeeVM)
        {
            var employee = context.Employees.Find(id);
            if (employee == null)
            {
                return RedirectToAction("Index", "Employee");
            }

            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = employee.Id;
                ViewData["JoiningDate"] = employee.JoiningDate.ToString("MM/dd/yyyy");

                return View(employeeVM);
            }

            string typeName = GetTypeName(employeeVM.JobDescription);

            // Update the product details
            employee.Name = employeeVM.Name;
            employee.JobDescription = typeName;
            employee.PhoneNumber = employeeVM.PhoneNumber;
            employee.EmailAddress = employeeVM.EmailAddress;
            employee.salary = employeeVM.salary;

            context.SaveChanges();
            return RedirectToAction("Index", "Employee");
        }


        public IActionResult Delete(int id)
        {
            var employee = context.Employees.Find(id);
            if (employee == null)
            {
                return RedirectToAction("Index", "Employee");
            }
            context.Employees.Remove(employee);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Employee");
        }
    }
}
