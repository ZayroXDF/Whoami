using Microsoft.EntityFrameworkCore;
using Whoami.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Whoami.Api.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Blogpost> Blogposts { get; set; }
}