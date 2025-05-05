using Ceng382_week5.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Ceng382_week5.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }
         public DbSet<User> Users { get; set; }
        public DbSet<ClassInformationModel> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

              modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Username); // Veya Id kullanıyorsanız ona göre ayarlayın
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.Password).HasMaxLength(100);
                entity.Property(u => u.Role).HasMaxLength(20);
            });
            
            modelBuilder.Entity<ClassInformationModel>(entity =>
            {
                entity.ToTable("Classes");
                entity.Property(p => p.ClassName).HasColumnName("Name");
                entity.Property(p => p.StudentCount).HasColumnName("PersonCount");
                entity.Property(p => p.Description).HasMaxLength(2000);
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var classes = new ClassInformationModel[100];
            for (int i = 1; i <= 100; i++)
            {
                classes[i-1] = new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"{GetRandomSubject()} {i}",
                    StudentCount = new Random().Next(15, 50),
                    Description = $"Description for {GetRandomSubject()} {i}",
                    IsActive = true
                };
            }
            
            modelBuilder.Entity<ClassInformationModel>().HasData(classes);
        }

        private string GetRandomSubject()
        {
            var subjects = new[] { "Math", "Physics", "Biology", "Chemistry", "History", 
                                 "Literature", "Computer Science", "Geography", "Art", "Music" };
            return subjects[new Random().Next(subjects.Length)];
        }
    }
}