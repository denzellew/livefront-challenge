using System.Security.Claims;
using carton_caps_referral.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carton_caps_referral.Controllers;

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
            return Unauthorized();
        }

        try
        {
            var referralCode = _referralService.GetUserReferralCode(Guid.Parse(userId));
            return Ok(referralCode);
        }
        catch (Exception ex)
        {
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
            return Unauthorized();
        }

        try
        {
            var referrals = _referralService.GetUserReferrals(Guid.Parse(userId));
            return Ok(referrals);
        }
        catch (Exception ex)
        {
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
            return Unauthorized();
        }

        try
        {
            var referralShortLink = _referralService.GenerateReferralShortLink(Guid.Parse(userId));
            return Ok(referralShortLink);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}