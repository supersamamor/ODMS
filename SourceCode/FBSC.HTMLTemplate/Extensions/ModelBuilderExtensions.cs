using FBSC.HTMLTemplate.Models;
using Microsoft.EntityFrameworkCore;
namespace FBSC.HTMLTemplate.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureHTMLTemplateEntities(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HTMLTemplateState>().ToTable("HTMLTemplate");
            modelBuilder.Entity<HTMLTemplateState>().HasIndex(p => p.HTMLTemplateName).IsUnique();
            modelBuilder.Entity<HTMLTemplateState>().Property(e => e.HTMLTemplateName).HasMaxLength(255);
            modelBuilder.Entity<HTMLTemplateState>().Property(e => e.Orientation).HasMaxLength(10);
            modelBuilder.Entity<HTMLTemplateState>().Property(e => e.PaperSize).HasMaxLength(10);
        }
    }
}
