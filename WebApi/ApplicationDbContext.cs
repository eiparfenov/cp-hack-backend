using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi;

public class ApplicationDbContext: DbContext
{
    public DbSet<BasicVideo> Videos { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var opts = new JsonSerializerOptions();
        modelBuilder.Entity<BasicVideo>(e =>
        {
            e.Property(p => p.Highlights).HasColumnType("jsonb").HasConversion(t => JsonSerializer.Serialize(t, opts), t => JsonSerializer.Deserialize<List<Highlight>>(t, opts)!);
        });
    }
}