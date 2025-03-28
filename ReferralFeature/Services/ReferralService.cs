using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using CartonCaps.ReferralFeature.Services.Interfaces;

namespace CartonCaps.ReferralFeature.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralCodeRepository _referralCodeRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly ILogger<ReferralService> _logger;
        public ReferralService(
            IReferralCodeRepository referralCodeRepository,
            IReferralRepository referralRepository,
            ILogger<ReferralService> logger
        )
        {
            _referralCodeRepository = referralCodeRepository;
            _referralRepository = referralRepository;
            _logger = logger;
        }

        public string GetUserReferralCode(Guid userId)
        {
            // Get the latest referral code for the user

            // If the user has no referral code, create a new one and save it

            // Return the referral code
            throw new NotImplementedException();
        }

        public List<Referral> GetUserReferrals(Guid userId)
        {
            // Get all referral codes with referrals for the user

            // Flatten the referrals

            // Return the referrals
            throw new NotImplementedException();
        }

        public string GenerateReferralShortLink(Guid userId)
        {
            // Get the referral code for the user

            // Generate a short link for the referral code

            // Return the short link
            throw new NotImplementedException();
        }

        public Referral CreateReferral(string referralCode, Guid referreeId)
        {
            // Get the referral code by code

            // Create a new referral

            // Save the referral

            // Return the referral
            throw new NotImplementedException();
        }

        public Referral CompleteReferral(string referralId)
        {
            // Get the referral by id

            // If none found, throw an exception

            // Update the referral status to completed

            // Save the referral

            // Return the referral
            throw new NotImplementedException();
        }
    }
}
