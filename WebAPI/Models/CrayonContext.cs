using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebAPI.Models.App;

namespace WebAPI.Models
{
    public class CrayonContext : IdentityDbContext<ApplicationUser>

    {
        public CrayonContext(DbContextOptions options) : base(options)
        {
            
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<IdentityRole>().HasData(
              new { Id = "1", Name = "GlobalAdmin", NormalizedName = "GLOBALADMIN" },
              //new { Id = "1", Name = "Achraf", NormalizedName = "Achraf" },
              new { Id = "2", Name = "Admin", NormalizedName = "ADMIN" }
             );

            //modelBuilder.Entity<ApplicationUser>().HasData(
                
            //    new
            //    {
            //        FirstName= "GlobalAdmin",l

            //    }
            //    )



            //  modelBuilder.EnableAutoHistory(null);
        }




        // Creating Roles for the application
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<IdentityRole>().HasData(
        //     new { Id = "1", Name = "GlobalAdmin", NormalizedName = "GLOBALADMIN" },
        //     new { Id = "2", Name = "Admin", NormalizedName = "ADMIN" }
        //    );


        //}

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
