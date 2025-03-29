using System.Security.Claims;
using CartonCaps.ReferralFeature.Controllers;
using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Dtos;
using CartonCaps.ReferralFeature.Integrations.Interfaces;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Models.Enums;
using CartonCaps.ReferralFeature.Repositories;
using CartonCaps.ReferralFeature.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CartonCaps.Tests.ReferralFeature.Controllers;

public class ReferralControllerTests : IDisposable
{
    private readonly DbContextOptions<ReferralDbContext> _dbContextOptions;
    private readonly ReferralDbContext _dbContext;
    private readonly Mock<IDeepLinkService> _deepLinkServiceMock;
    private readonly Mock<IReferralCodeGenerator> _referralCodeGeneratorMock;
    private readonly Mock<ILogger<ReferralController>> _loggerMock;
    private readonly ReferralController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public ReferralControllerTests()
    {
        // Setup in-memory database
        _dbContextOptions = new DbContextOptionsBuilder<ReferralDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ReferralDbContext(_dbContextOptions);

        // Setup mocks
        _deepLinkServiceMock = new Mock<IDeepLinkService>();
        _referralCodeGeneratorMock = new Mock<IReferralCodeGenerator>();
        _loggerMock = new Mock<ILogger<ReferralController>>();

        var referralCodeRepository = new ReferralCodeRepository(_dbContext);
        var referralRepository = new ReferralRepository(_dbContext);
        var mockLogger = new Mock<ILogger<ReferralService>>();

        // Setup ReferralService with real implementation
        var referralService = new ReferralService(
            referralCodeRepository,
            referralRepository,
            _deepLinkServiceMock.Object,
            _referralCodeGeneratorMock.Object,
            mockLogger.Object
        );

        // Setup controller with mock user
        _controller = new ReferralController(_loggerMock.Object, referralService);
        SetupControllerUser();
    }

    private void SetupControllerUser()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetReferralCode_WhenUserDoesNotHaveCode_GeneratesNewCode()
    {
        // Arrange
        var expectedCode = "TEST123";
        _referralCodeGeneratorMock.Setup(x => x.GenerateReferralCode())
            .Returns(expectedCode);

        // Act
        var result = await _controller.GetReferralCode();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCode = Assert.IsType<ReferralCodeDto>(okResult.Value);
        Assert.Equal(expectedCode, returnedCode.Code);

        // Verify code was saved to database
        var savedCode = await _dbContext.ReferralCodes
            .FirstOrDefaultAsync(rc => rc.UserId == _testUserId);
        Assert.NotNull(savedCode);
        Assert.Equal(expectedCode, savedCode.Code);
    }

    [Fact]
    public async Task GetReferralCode_WhenUserHasExistingCode_ReturnsExistingCode()
    {
        // Arrange
        var existingCode = "EXISTING123";
        _dbContext.ReferralCodes.Add(new ReferralCode()
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            Code = existingCode,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetReferralCode();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCode = Assert.IsType<ReferralCodeDto>(okResult.Value);
        Assert.Equal(existingCode, returnedCode.Code);
        _referralCodeGeneratorMock.Verify(x => x.GenerateReferralCode(), Times.Never);
    }

    [Fact]
    public async Task GetReferrals_ReturnsUserReferrals()
    {
        // Arrange
        var referralCode = new ReferralCode
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            Code = "TEST123",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ReferralCodes.Add(referralCode);

        var referral = new Referral()
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
        var result = await _controller.GetReferrals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedReferrals = Assert.IsType<List<ReferralDto>>(okResult.Value);
        Assert.Single(returnedReferrals);
        Assert.Equal(referral.RefereeId.ToString(), returnedReferrals[0].RefereeId);
        Assert.Equal(referral.Status.ToString(), returnedReferrals[0].Status);
    }

    [Fact]
    public async Task GenerateReferralShortLink_GeneratesAndReturnsLink()
    {
        // Arrange
        var expectedCode = "TEST123";
        var expectedLink = "https://cartoncaps.link/abc123";

        _dbContext.ReferralCodes.Add(new ReferralCode
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            Code = expectedCode,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        _deepLinkServiceMock.Setup(x => x.GenerateDeepLinkAsync(expectedCode))
            .ReturnsAsync(expectedLink);

        // Act
        var result = await _controller.GenerateReferralShortLink();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedLink = Assert.IsType<ReferralLinkDto>(okResult.Value);
        Assert.Equal(expectedLink, returnedLink.Link);
        _deepLinkServiceMock.Verify(x => x.GenerateDeepLinkAsync(expectedCode), Times.Once);
    }
}