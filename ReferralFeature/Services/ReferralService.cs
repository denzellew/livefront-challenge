using System;
using carton_caps_referral.Models;
using carton_caps_referral.Services.Interfaces;

namespace carton_caps_referral.Services
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
