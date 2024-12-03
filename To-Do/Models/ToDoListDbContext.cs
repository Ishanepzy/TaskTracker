using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace To_Do.Models
{
    public class ToDoListDbContext : IdentityDbContext<ApplicationUser>
    {
        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options)
        {
        }

        public DbSet<UserTask> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserTask>()
                .HasOne(td => td.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(td => td.UserId);
        }
    }
}
