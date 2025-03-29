using System.Security.Claims;
using CartonCaps.ReferralFeature.Dtos;
using CartonCaps.ReferralFeature.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.ReferralFeature.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<ReferralCodeDto>> GetReferralCode()
    {
        var userId = GetUserId();

        try
        {
            _logger.LogInformation("Getting referral code for user {UserId}", userId);
            var referralCode = await _referralService.GetUserReferralCode(userId);
            _logger.LogInformation("Successfully retrieved referral code for user {UserId}", userId);
            return Ok(new ReferralCodeDto { Code = referralCode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting referral code for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("referrals", Name = "GetReferrals")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> GetReferrals()
    {
        var userId = GetUserId();

        try
        {
            _logger.LogInformation("Getting referrals for user {UserId}", userId);
            var referrals = await _referralService.GetUserReferrals(userId);
            _logger.LogInformation("Successfully retrieved referrals for user {UserId}", userId);

            var result = referrals.Select(r => new ReferralDto
            {
                Id = r.Id.ToString(),
                RefereeId = r.RefereeId.ToString(),
                Status = r.Status.ToString(),
                CompletedAt = r.CompletedAt ?? DateTime.MinValue
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting referrals for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("link", Name = "GenerateReferralShortLink")]
    [Authorize]
    public async Task<ActionResult<ReferralLinkDto>> GenerateReferralShortLink()
    {
        var userId = GetUserId();

        try
        {
            _logger.LogInformation("Generating referral short link for user {UserId}", userId);
            var referralShortLink = await _referralService.GenerateReferralShortLink(userId);
            _logger.LogInformation("Successfully generated referral short link for user {UserId}", userId);
            return Ok(new ReferralLinkDto { Link = referralShortLink });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating referral short link for user {UserId}", userId);
            return StatusCode(500, ex.Message);
        }
    }

    private Guid GetUserId()
    {
        var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new UnauthorizedAccessException("User ID not found");
        return Guid.Parse(userId);
    }
}