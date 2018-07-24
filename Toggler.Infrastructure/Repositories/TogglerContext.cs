using Microsoft.EntityFrameworkCore;
using Toggler.Domain.Entities;

namespace Toggler.Infrastructure.Repositories
{
    public class TogglerContext : DbContext
    {
        public TogglerContext(DbContextOptions<TogglerContext> options)
            : base(options)
        {
        }

        // Add entities
        public DbSet<Toggle> Toggles { get; set; } 
        public DbSet<ServiceToggle> ServiceToggles { get; set; }
    }
}
