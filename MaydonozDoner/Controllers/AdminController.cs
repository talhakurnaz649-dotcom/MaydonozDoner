using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaydonozDoner.Data;
using MaydonozDoner.Models;
using System.Dynamic;

namespace MaydonozDoner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            int employeeCount = await _context.Employees.CountAsync();
            int productCount = await _context.Products.CountAsync();
            int userCount = await _userManager.Users.CountAsync();

            var users = await _userManager.Users.ToListAsync();
            var userRolesList = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesList.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    Username = user.UserName ?? "Bilinmiyor",
                    Email = user.Email ?? "Bilinmiyor",
                    Role = string.Join(", ", roles)
                });
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.EmployeeCount = employeeCount;
            viewModel.ProductCount = productCount;
            viewModel.UserCount = userCount;
            viewModel.Users = userRolesList;
            
            viewModel.MonthlyRevenue = 245850.00;
            viewModel.ActiveBranches = 14;
            viewModel.TotalOrders = 1845;
            viewModel.BestSeller = "🌿 Maydonoz Tavuk Döner";

            return View(viewModel);
        }
    }

    public class UserRoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
