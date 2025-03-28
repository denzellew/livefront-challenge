using System.Security.Claims;
using CartonCaps.ReferralFeature.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.ReferralFeature.Controllers;

[ApiController]
[Route("[controller]")]
public class ReferralController : ControllerBase
{
    private readonly ILogger<ReferralController> _logger;
    private readonly IReferralService _referralService;

    public ReferralController(ILogger<ReferralController> logger, IReferralService referralService)
    {
        _logger = logger;
        _referralService = referralService;
    }

    [HttpGet("code", Name = "GetReferralCode")]
    [Authorize]
    public IActionResult GetReferralCode()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to GetReferralCode");
            return Unauthorized();
        }

        try
        {
            _logger.LogInformation("Getting referral code for user {UserId}", userId);
            var referralCode = _referralService.GetUserReferralCode(Guid.Parse(userId));
            _logger.LogInformation("Successfully retrieved referral code for user {UserId}", userId);
            return Ok(referralCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting referral code for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("referrals", Name = "GetReferrals")]
    [Authorize]
    public IActionResult GetReferrals()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to GetReferrals");
            return Unauthorized();
        }

        try
        {
            _logger.LogInformation("Getting referrals for user {UserId}", userId);
            var referrals = _referralService.GetUserReferrals(Guid.Parse(userId));
            _logger.LogInformation("Successfully retrieved referrals for user {UserId}", userId);
            return Ok(referrals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting referrals for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("link", Name = "GenerateReferralShortLink")]
    [Authorize]
    public IActionResult GenerateReferralShortLink()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to GenerateReferralShortLink");
            return Unauthorized();
        }

        try
        {
            _logger.LogInformation("Generating referral short link for user {UserId}", userId);
            var referralShortLink = _referralService.GenerateReferralShortLink(Guid.Parse(userId));
            _logger.LogInformation("Successfully generated referral short link for user {UserId}", userId);
            return Ok(referralShortLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating referral short link for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }
}