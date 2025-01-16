using GetMyThingsWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMyThingsWeb.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
    }
}
