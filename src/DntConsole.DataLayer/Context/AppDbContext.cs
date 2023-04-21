using DntConsole.Entities;
using Microsoft.EntityFrameworkCore;

namespace DntConsole.DataLayer.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts { get; set; } = default!;
}