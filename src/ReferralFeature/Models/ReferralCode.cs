using System;

namespace CartonCaps.ReferralFeature.Models;

public class ReferralCode
{
    public ReferralCode()
    {
        Referrals = [];
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public DateTime CreatedAt { get; init; }

    public virtual ICollection<Referral> Referrals { get; set; } = [];
}
