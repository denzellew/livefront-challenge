using CartonCaps.ReferralFeature.Models.Enums;

namespace CartonCaps.ReferralFeature.Models;

public class Referral
{
    public required Guid Id { get; set; }
    public required Guid ReferralCodeId { get; set; }

    public required Guid RefereeId { get; set; }

    public ReferralStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ReferralCode? ReferralCode { get; set; }
}

