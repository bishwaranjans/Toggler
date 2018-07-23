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
        public DbSet<Toggle> Toggles { get; set; } //TODO: Remove this model as ServiceToggle contains all the necessary properties
        public DbSet<ServiceToggle> ServiceToggles { get; set; }
    }
}
