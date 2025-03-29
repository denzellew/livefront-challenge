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

        modelBuilder.Entity<ReferralCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasMany(e => e.Referrals).WithOne(e => e.ReferralCode).HasForeignKey(e => e.ReferralCodeId);
        });


        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RefereeId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasOne(e => e.ReferralCode).WithMany(e => e.Referrals).HasForeignKey(e => e.ReferralCodeId);
        });
    }
}