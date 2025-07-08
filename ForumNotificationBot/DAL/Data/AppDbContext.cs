using ForumNotificationBot.BLL.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumNotificationBot.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<NotificationEntity> NotificationMessages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationEntity>().ToTable("NotificationMessages");
            base.OnModelCreating(modelBuilder);
        }
    }
}
