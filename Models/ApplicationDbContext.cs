using DotNetCoreAssignments.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ToDo> ToDo { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder.Entity<ToDo>().HasKey(t => t.Id);
        SeedRoles(builder);
    }

    private void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
            new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
            new IdentityRole() { Name = "User1", ConcurrencyStamp = "3", NormalizedName = "User1" }
        );
    }
}
