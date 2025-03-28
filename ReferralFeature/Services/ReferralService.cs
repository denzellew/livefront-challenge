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

        public async Task<string> GetUserReferralCode(Guid userId)
        {
            // Get the latest referral code for the user
            var referralCode = await _referralCodeRepository.GetLatestByUserIdAsync(userId);

            // If the user has no referral code, create a new one and save it
            if (referralCode == null)
            {
                var code = "";
                for (int attempts = 0; attempts < 10; attempts++)
                {
                    // Generate Code
                    code = Guid.NewGuid().ToString().Substring(0, 6);

                    // Check if the code is unique
                    var existingCode = await _referralCodeRepository.GetByCodeAsync(code);

                    // If the code is unique, break the loop
                    if (existingCode == null)
                    {
                        break;
                    }

                    // If the code is not unique , try again
                }

                if (code == "")
                {
                    throw new Exception("Failed to generate a unique referral code");
                }

                var newReferralCode = new ReferralCode()
                {
                    Code = code,
                    UserId = userId
                };

                referralCode = await _referralCodeRepository.AddAsync(newReferralCode);
            }

            // Return the referral code
            return referralCode.Code;
        }

        public async Task<List<Referral>> GetUserReferrals(Guid userId)
        {
            // Get all referral codes with referrals for the user
            var referralCodes = await _referralCodeRepository.GetAllUserReferralsAsync(userId);

            // Flatten the referrals
            var referrals = referralCodes.SelectMany(code => code.Referrals).ToList();

            // Return the referrals
            return referrals;
        }

        public async Task<string> GenerateReferralShortLink(Guid userId)
        {
            // Get the referral code for the user

            // Generate a short link for the referral code

            // Return the short link
            throw new NotImplementedException();
        }

        public async Task<Referral> CreateReferral(string referralCode, Guid referreeId)
        {
            // Get the referral code by code

            // Create a new referral

            // Save the referral

            // Return the referral
            throw new NotImplementedException();
        }

        public async Task<Referral> CompleteReferral(string referralId)
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
