using System;
using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralFeature.Repositories;

public class ReferralCodeRepository : BaseRepository<ReferralCode>, IReferralCodeRepository
{
    public ReferralCodeRepository(ReferralDbContext context) : base(context)
    {
    }

    public async Task<ReferralCode?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<ReferralCode?> GetLatestByUserIdAsync(Guid userId)
    {
        // Get the latest referral code for the user
        return await _dbSet.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<IEnumerable<ReferralCode>> GetAllUserReferralsAsync(Guid userId)
    {
        return await _dbSet.Include(x => x.Referrals).Where(x => x.UserId == userId).ToListAsync();
    }
}
