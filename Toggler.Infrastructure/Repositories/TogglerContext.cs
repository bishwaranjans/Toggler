using Microsoft.EntityFrameworkCore;
using Toggler.Domain.Entities;

namespace Toggler.Infrastructure.Repositories
{
    /// <summary>
    /// Toggler context
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class TogglerContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TogglerContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public TogglerContext(DbContextOptions<TogglerContext> options)
            : base(options)
        {
        }

        // Add entities
        public DbSet<Toggle> Toggles { get; set; } 
        public DbSet<ServiceToggle> ServiceToggles { get; set; }
    }
}
