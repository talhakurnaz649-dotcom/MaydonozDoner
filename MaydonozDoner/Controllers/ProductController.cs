using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaydonozDoner.Data;
using MaydonozDoner.Models;

namespace MaydonozDoner.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
    }
}
