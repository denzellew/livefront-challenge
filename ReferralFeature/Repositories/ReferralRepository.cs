using System;
using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralFeature.Repositories;

public class ReferralRepository : BaseRepository<Referral>, IReferralRepository
{
    public ReferralRepository(ReferralDbContext context) : base(context)
    {
    }
}
