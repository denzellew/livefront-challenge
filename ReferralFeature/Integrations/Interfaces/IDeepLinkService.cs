using System;

namespace CartonCaps.ReferralFeature.Integrations.Interfaces;

public interface IDeepLinkService
{
    Task<string> GenerateDeepLinkAsync(string referralCode);
}
