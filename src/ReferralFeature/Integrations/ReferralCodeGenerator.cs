using CartonCaps.ReferralFeature.Integrations.Interfaces;

namespace CartonCaps.ReferralFeature.Integrations;

public class ReferralCodeGenerator : IReferralCodeGenerator
{
    public string GenerateReferralCode()
    {
        return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
    }
}
