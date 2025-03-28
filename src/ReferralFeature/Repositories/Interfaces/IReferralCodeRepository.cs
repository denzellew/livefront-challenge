using CartonCaps.ReferralFeature.Models;

namespace CartonCaps.ReferralFeature.Repositories.Interfaces;

public interface IReferralCodeRepository : IBaseRepository<ReferralCode>
{
    public Task<ReferralCode?> GetByCodeAsync(string code);
    public Task<ReferralCode?> GetLatestByUserIdAsync(Guid userId);
    public Task<IEnumerable<ReferralCode>> GetAllUserReferralsAsync(Guid userId);
}
