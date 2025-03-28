using System;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralFeature.Repositories;

public class ReferralRepository : BaseRepository<Referral>, IReferralRepository
{
    public ReferralRepository(DbContext context) : base(context)
    {
    }
}
