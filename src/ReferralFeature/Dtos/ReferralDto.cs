namespace CartonCaps.ReferralFeature.Dtos
{
    public class ReferralDto
    {
        public required string Id { get; set; }
        public required string RefereeId { get; set; }
        public required string Status { get; set; }
        public required DateTime CompletedAt { get; set; }
    }
}
