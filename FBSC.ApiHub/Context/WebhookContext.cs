using FBSC.ApiHub.Models;
using FBSC.Common.Data;
using FBSC.Common.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ApiHub.Context
{
    public class WebhookContext(DbContextOptions<WebhookContext> options, IAuthenticatedUser authenticatedUser) 
        : AuditableDbContext<WebhookContext>(options, authenticatedUser)
    {
        public DbSet<WebhookApiState> WebhookApi { get; set; } = default!;
        public DbSet<WebhookEventAssignmentState> WebhookEventAssignment { get; set; } = default!;
        public DbSet<WebhookLogsState> WebhookLogs { get; set; } = default!;

        // Optional: Override OnModelCreating to configure entity mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations here
        }
    }
}
