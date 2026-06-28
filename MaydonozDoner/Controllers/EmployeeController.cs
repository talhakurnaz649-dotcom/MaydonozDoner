using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaydonozDoner.Data;
using MaydonozDoner.Models;

namespace MaydonozDoner.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.Include(e => e.Department).ToListAsync();
            return Json(employees);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Çalışan başarıyla eklendi!" });
            }
            return Json(new { success = false, message = "Veri doğrulanamadı." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Çalışan silindi!" });
            }
            return Json(new { success = false, message = "Çalışan bulunamadı." });
        }
    }
}
