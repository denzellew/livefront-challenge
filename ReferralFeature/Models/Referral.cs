using carton_caps_referral.Models.Enums;

namespace carton_caps_referral.Models
{
    public class Referral
    {
        public required string Id { get; set; }
        public required string ReferrerId { get; set; }

        public required string ReferreeId { get; set; }

        public ReferralStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
