using CartonCaps.ReferralFeature.Models;

namespace CartonCaps.ReferralFeature.Services.Interfaces
{
    public interface IReferralService
    {

        Task<string> GetUserReferralCode(Guid userId);

        Task<List<Referral>> GetUserReferrals(Guid userId);

        Task<string> GenerateReferralShortLink(Guid userId);

        Task<Referral> CreateReferral(string referralCode, Guid referreeId);

        Task<Referral> CompleteReferral(string referralId);
    }
}
