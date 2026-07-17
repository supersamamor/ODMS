using FBSC.Common.Data;
using FBSC.Common.Identity.Abstractions;
using FBSC.HTMLTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace FBSC.HTMLTemplate.Context
{
    public class HTMLTemplateContext(DbContextOptions<HTMLTemplateContext> options, IAuthenticatedUser authenticatedUser) 
        : AuditableDbContext<HTMLTemplateContext>(options, authenticatedUser)
    {
        public DbSet<HTMLTemplateState> HTMLTemplate { get; set; } = default!;

        // Optional: Override OnModelCreating to configure entity mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations here
        }
    }
}
