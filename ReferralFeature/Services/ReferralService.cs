using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Services.Interfaces;

namespace CartonCaps.ReferralFeature.Services
{
    public class ReferralService : IReferralService
    {
        public string GetUserReferralCode(Guid userId)
        {
            throw new NotImplementedException();
        }

        public List<Referral> GetUserReferrals(Guid userId)
        {
            throw new NotImplementedException();
        }

        public string GenerateReferralShortLink(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Referral CompleteReferral(string referralId)
        {
            throw new NotImplementedException();
        }

        public Referral CreateReferral(string referralCode, Guid referreeId)
        {
            throw new NotImplementedException();
        }
    }
}
