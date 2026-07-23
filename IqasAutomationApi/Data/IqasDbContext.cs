using IqasAutomationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IqasAutomationApi.Data;

public class IqasDbContext : DbContext
{
    public IqasDbContext(DbContextOptions<IqasDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Defect> Defects => Set<Defect>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audit>()
            .HasOne(a => a.Location)
            .WithMany(l => l.Audits)
            .HasForeignKey(a => a.LocationId);

        modelBuilder.Entity<Defect>()
            .HasOne(d => d.Audit)
            .WithMany(a => a.Defects)
            .HasForeignKey(d => d.AuditId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}