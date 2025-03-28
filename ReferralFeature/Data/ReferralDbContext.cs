using CartonCaps.ReferralFeature.Models;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralFeature.Data;

public class ReferralDbContext : DbContext
{
    public ReferralDbContext(DbContextOptions<ReferralDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReferralCode> ReferralCodes { get; set; }
    public DbSet<Referral> Referrals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add any entity configurations here
        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Add other configurations as needed
        });
    }
}