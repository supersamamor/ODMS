using FBSC.ApiHub.Models;
using Microsoft.EntityFrameworkCore;


namespace FBSC.ApiHub.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureApiHubEntities(this ModelBuilder modelBuilder)
        {       
            modelBuilder.Entity<WebhookApiState>().ToTable("WebhookApi");
            modelBuilder.Entity<WebhookEventAssignmentState>().ToTable("WebhookEventAssignment");
            modelBuilder.Entity<WebhookLogsState>().ToTable("WebhookLogs");
            modelBuilder.Entity<WebhookApiState>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<WebhookEventAssignmentState>().HasIndex(p => new { p.EventName, p.WebhookApiId }).IsUnique();
            modelBuilder.Entity<WebhookApiState>().Property(e => e.Name).HasMaxLength(50);
            modelBuilder.Entity<WebhookApiState>().Property(e => e.GrantType).HasMaxLength(20);
            modelBuilder.Entity<WebhookApiState>().Property(e => e.ClientSecret).HasMaxLength(450);          
            modelBuilder.Entity<WebhookApiState>().Property(e => e.BaseUrl).HasMaxLength(255);
            modelBuilder.Entity<WebhookApiState>().Property(e => e.AuthenticationUrl).HasMaxLength(255);
            modelBuilder.Entity<WebhookEventAssignmentState>().Property(e => e.EventName).HasMaxLength(255);
            modelBuilder.Entity<WebhookEventAssignmentState>().Property(e => e.Route).HasMaxLength(255);
            modelBuilder.Entity<WebhookEventAssignmentState>().Property(e => e.Method).HasMaxLength(15);
            modelBuilder.Entity<WebhookApiState>().HasMany(t => t.WebhookEventAssignmentList).WithOne(l => l.WebhookApi).HasForeignKey(t => t.WebhookApiId);
            modelBuilder.Entity<WebhookEventAssignmentState>().HasMany(t => t.WebhookLogsList).WithOne(l => l.WebhookEventAssignment).HasForeignKey(t => t.WebhookEventAssignmentId);
            modelBuilder.Entity<WebhookLogsState>().Property(e => e.Status).HasMaxLength(25);
            modelBuilder.Entity<WebhookLogsState>().Property(e => e.DataId).HasMaxLength(36);
            modelBuilder.Entity<WebhookLogsState>().HasIndex(p => p.DataId);
            modelBuilder.Entity<WebhookLogsState>().Property(e => e.ParametarizedRoute).HasMaxLength(450);


            modelBuilder.Entity<WebhookEventAssignmentState>()
            .Property(e => e.Active)
            .HasDefaultValue(true);

            modelBuilder.Entity<WebhookApiState>()
                .Property(e => e.Active)
                .HasDefaultValue(true);

            // Configure the filtered index for Quartz polling performance
            modelBuilder.Entity<WebhookLogsState>()
                .HasIndex(w => w.CreatedDate, "IX_WebhookLogs_PendingJobs")
                .IncludeProperties(w => new
                {
                    w.WebhookEventAssignmentId,
                    w.DataId,
                    w.Payload,
                    w.ParametarizedRoute
                })
                .HasFilter($"[{nameof(WebhookLogsState.Status)}] = '${WebhookStatus.Pending}'");
        }
    }
}
