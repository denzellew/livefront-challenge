using CartonCaps.ReferralFeature.Integrations.Interfaces;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using CartonCaps.ReferralFeature.Services.Interfaces;

namespace CartonCaps.ReferralFeature.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralCodeRepository _referralCodeRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly IDeepLinkService _deepLinkService;
        private readonly IReferralCodeGenerator _referralCodeGenerator;
        private readonly ILogger<ReferralService> _logger;
        public ReferralService(
            IReferralCodeRepository referralCodeRepository,
            IReferralRepository referralRepository,
            IDeepLinkService deepLinkService,
            IReferralCodeGenerator referralCodeGenerator,
            ILogger<ReferralService> logger
        )
        {
            _referralCodeRepository = referralCodeRepository;
            _referralRepository = referralRepository;
            _deepLinkService = deepLinkService;
            _referralCodeGenerator = referralCodeGenerator;
            _logger = logger;
        }

        public async Task<string> GetUserReferralCode(Guid userId)
        {
            _logger.LogInformation("Getting referral code for user {UserId}", userId);

            var referralCode = await _referralCodeRepository.GetLatestByUserIdAsync(userId);

            if (referralCode == null)
            {
                _logger.LogInformation("No existing referral code found for user {UserId}, generating new code", userId);
                var code = "";

                for (int attempts = 0; attempts < 10; attempts++)
                {
                    code = _referralCodeGenerator.GenerateReferralCode();
                    _logger.LogDebug("Generated referral code attempt {Attempt}: {Code}", attempts + 1, code);

                    var existingCode = await _referralCodeRepository.GetByCodeAsync(code);

                    if (existingCode == null)
                    {
                        break;
                    }
                    _logger.LogDebug("Generated code {Code} already exists, retrying", code);
                }

                if (code == "")
                {
                    _logger.LogError("Failed to generate unique referral code for user {UserId} after 10 attempts", userId);
                    throw new Exception("Failed to generate a unique referral code");
                }

                var newReferralCode = new ReferralCode()
                {
                    Code = code,
                    UserId = userId
                };

                referralCode = await _referralCodeRepository.AddAsync(newReferralCode);
                _logger.LogInformation("Created new referral code {Code} for user {UserId}", code, userId);
            }

            return referralCode.Code;
        }

        public async Task<List<Referral>> GetUserReferrals(Guid userId)
        {
            _logger.LogInformation("Retrieving referrals for user {UserId}", userId);

            var referralCodes = await _referralCodeRepository.GetAllUserReferralsAsync(userId);
            var referrals = referralCodes.SelectMany(code => code.Referrals).ToList();

            _logger.LogInformation("Found {Count} referrals for user {UserId}", referrals.Count, userId);
            return referrals;
        }

        public async Task<string> GenerateReferralShortLink(Guid userId)
        {
            _logger.LogInformation("Generating referral short link for user {UserId}", userId);

            var referralCode = await GetUserReferralCode(userId);

            if (referralCode == null)
            {
                _logger.LogError("Failed to generate referral code for user {UserId}", userId);
                throw new Exception("Failed to generate a referral code");
            }

            var deepLink = await _deepLinkService.GenerateDeepLinkAsync(referralCode);
            _logger.LogInformation("Generated deep link for user {UserId} with code {Code}", userId, referralCode);

            return deepLink;
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
