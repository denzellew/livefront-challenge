using System;
using CartonCaps.ReferralFeature.Integrations.Interfaces;

namespace CartonCaps.ReferralFeature.Integrations;

public class MockDeepLinkService : IDeepLinkService
{
    public Task<string> GenerateDeepLinkAsync(string referralCode)
    {
        return Task.FromResult($"https://cartoncaps.link/abfilefa90p?referralCode={referralCode}");
    }
}
