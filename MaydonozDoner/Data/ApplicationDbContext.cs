using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MaydonozDoner.Models;
using Microsoft.Extensions.Options;
namespace MaydonozDoner.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base (options){}
        
        public DbSet<Employee> Employees { get; set; } 
        public DbSet<Department> Departments { get; set; } 
        public DbSet<Salary> Salaries { get; set; } 
        public DbSet<Product> Products { get; set; } 

    }
}
