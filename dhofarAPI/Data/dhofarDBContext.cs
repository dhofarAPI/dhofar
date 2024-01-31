using dhofarAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace dhofarAPI.Data
{
    public class dhofarDBContext: IdentityDbContext<User>
    {
        public dhofarDBContext(DbContextOptions<dhofarDBContext> options) : base(options)
        {

        }
        public DbSet<Complaint> Complaints { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<ComplaintsFile> ComplaintsFiles { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<SubjectType> SubjectTypes { get; set; }

        public DbSet<SubjectFiles> SubjectFiles { get; set; }

        public DbSet<RatingSubject> RatingSubjects { get; set; }

        public DbSet<CommentSubject> CommentSubjects { get; set; }

        public DbSet<FavoriteSubject> FavoriteSubjects { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FavoriteSubject>()
                 .HasKey(fs => new { fs.UserId, fs.SubjectId });

            builder.Entity<FavoriteSubject>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.FavoriteSubjects)
                .HasForeignKey(fs => fs.UserId)
                .IsRequired()  // Make UserId a required field
                .OnDelete(DeleteBehavior.Restrict); // Specify NO ACTION

            builder.Entity<FavoriteSubject>()
                .HasOne(fs => fs.Subject)
                .WithMany(s => s.FavoriteSubjects)
                .HasForeignKey(fs => fs.SubjectId)
                .IsRequired()  // Make SubjectId a required field
                .OnDelete(DeleteBehavior.Cascade); // Specify NO ACTION

            // Set the length of UserId to match the length in AspNetUsers table
            builder.Entity<FavoriteSubject>()
                .Property(fs => fs.UserId)
                .HasMaxLength(450); // Adjust the length as needed

            // setup the keys for the comment suvbject table 
            builder.Entity<CommentSubject>()
                .HasKey(cs => cs.Id);

            builder.Entity<CommentSubject>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CommentSubjects)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommentSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.CommentSubjects)
                .HasForeignKey(cs => cs.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);


            // setup the keys for the Rating subject table 
            builder.Entity<RatingSubject>()
                .HasKey(cs => cs.Id);

            builder.Entity<RatingSubject>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.RatingSubjects)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RatingSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.RatingSubjects)
                .HasForeignKey(cs => cs.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
                // Add more categories as needed
            );

            // Seed Complaints
            //builder.Entity<Complaint>().HasData(
            //    new Complaint
            //    {
            //        Id = 1,
            //        UserId="user1",
            //        State = "Slaleh",
            //        Type = "General",
            //        Title = "Noisy Neighbors",
            //        Description = "Neighbors are making too much noise late at night.",
            //        Status = "Open",
            //        Location = "123 Main Street",
            //        IsAccepted = false,
            //        Time = DateTime.UtcNow,
            //        CategoryId = 1
            //    },
            //    new Complaint
            //    {
            //        Id = 2,
            //        UserId = "user2",
            //        State = "dhofar",
            //        Type = "General",
            //        Title = "Trash not collected",
            //        Description = "Trash has not been collected for the past week.",
            //        Status = "Open",
            //        Location = "456 Elm Street",
            //        IsAccepted = false,
            //        Time = DateTime.UtcNow,
            //        CategoryId = 2
            //    }
            //    // Add more complaints as needed
            //);

            seedRole(builder, "Super Admin", "Create", "Update", "Delete", "Read");
            seedRole(builder, "Admin", "Create", "Update", "Delete", "Read");
            seedRole(builder, "User", "Create", "Update", "Delete", "Read");

        }
        int nextId = 1;
        private void seedRole(ModelBuilder modelBuilder, string roleName, params string[] permissions)
        {
            var role = new IdentityRole
            {
                Id = roleName.ToLower(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                ConcurrencyStamp = Guid.Empty.ToString()
            };
            var roleClaim = permissions.Select(permissions =>
            new IdentityRoleClaim<string>
            {
                Id = nextId++,
                RoleId = role.Id,
                ClaimType = "permissions",
                ClaimValue = permissions
            }).ToArray();
            modelBuilder.Entity<IdentityRole>().HasData(role);

        }
    }
}
