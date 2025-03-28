using System;
using carton_caps_referral.Models;

namespace carton_caps_referral.Services.Interfaces
{
    public interface IReferralService
    {

        string GetUserReferralCode(Guid userId);

        List<Referral> GetUserReferrals(Guid userId);

        string GenerateReferralShortLink(Guid userId);

        Referral CreateReferral(string referralCode, Guid referreeId);

        Referral CompleteReferral(string referralId);
    }
}
