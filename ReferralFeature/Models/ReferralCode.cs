using System;

namespace CartonCaps.ReferralFeature.Models;

public class ReferralCode
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required DateTime CreatedAt { get; set; }

    public virtual ICollection<Referral> Referrals { get; set; } = [];
}
