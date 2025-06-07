using Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class ViaTabloidDbContext : DbContext
    {
        public DbSet<Story> Stories => Set<Story>();
        public DbSet<Department> Departments => Set<Department>();

        public ViaTabloidDbContext(DbContextOptions<ViaTabloidDbContext> options)
            : base(options) { }

        public ViaTabloidDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasKey(d => d.Id);
            modelBuilder.Entity<Story>().HasKey(s => s.Id);

            modelBuilder.Entity<Story>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Stories)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // inser ´´ting
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Engineering" },
                new Department { Id = 2, Name = "Design" },
                new Department { Id = 3, Name = "Architecture" }
            );
        }
    }
}