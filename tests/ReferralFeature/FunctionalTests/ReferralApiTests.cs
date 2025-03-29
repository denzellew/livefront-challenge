using System.Net;
using System.Net.Http.Json;
using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Dtos;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Tests.ReferralFeature.FunctionalTests;
public class ReferralControllerTests : IClassFixture<ReferralFeatureTestFactory>
{
    private readonly ReferralFeatureTestFactory _factory;
    private readonly HttpClient _client;
    private readonly ReferralDbContext _dbContext;
    private readonly Guid _userId;

    public ReferralControllerTests(ReferralFeatureTestFactory factory)
    {
        _userId = Guid.NewGuid();
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient(_userId);
        _dbContext = _factory.GetDbContext();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task GetReferralCode_WhenUserDoesNotHaveCode_GeneratesNewCode()
    {
        // Act
        var response = await _client.GetAsync("/api/referral/code");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ReferralCodeDto>();
        Assert.NotNull(result);
        Assert.Equal("TEST123", result.Code);

        // Verify database
        var savedCode = await _dbContext.ReferralCodes.FirstOrDefaultAsync();
        Assert.NotNull(savedCode);
        Assert.Equal("TEST123", savedCode.Code);
    }

    [Fact]
    public async Task GetReferralCode_WhenUserHasExistingCode_ReturnsExistingCode()
    {
        // Arrange
        var existingCode = "EXISTING123";
        _dbContext.ReferralCodes.Add(new ReferralCode
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Code = existingCode,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/referral/code");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ReferralCodeDto>();
        Assert.NotNull(result);
        Assert.Equal(existingCode, result.Code);
    }

    [Fact]
    public async Task GetReferrals_ReturnsUserReferrals()
    {
        // Arrange
        var referralCode = new ReferralCode
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Code = "TEST123",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ReferralCodes.Add(referralCode);

        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferralCodeId = referralCode.Id,
            RefereeId = Guid.NewGuid(),
            Status = ReferralStatus.Completed,
            CompletedAt = DateTime.UtcNow
        };
        _dbContext.Referrals.Add(referral);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/referral");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ReferralDto>>();
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(referral.RefereeId.ToString(), result[0].RefereeId);
        Assert.Equal(referral.Status.ToString(), result[0].Status);
    }

    [Fact]
    public async Task GenerateReferralShortLink_GeneratesAndReturnsLink()
    {
        // Arrange
        var referralCode = new ReferralCode
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Code = "TEST123",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ReferralCodes.Add(referralCode);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/referral/short-link");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ReferralLinkDto>();
        Assert.NotNull(result);
        Assert.Equal("https://cartoncaps.link/abc123", result.Link);
    }

    [Fact]
    public async Task GenerateReferralShortLink_WithoutReferralCode_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/referral/short-link");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ... other tests ...
}