using CartonCaps.ReferralFeature.Integrations.Interfaces;
using CartonCaps.ReferralFeature.Models;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using CartonCaps.ReferralFeature.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCaps.Tests.ReferralFeature.Services
{
    public class ReferralServiceTests
    {
        private readonly Mock<IReferralCodeRepository> _mockReferralCodeRepository;
        private readonly Mock<IReferralRepository> _mockReferralRepository;
        private readonly Mock<IDeepLinkService> _mockDeepLinkService;
        private readonly Mock<IReferralCodeGenerator> _mockReferralCodeGenerator;
        private readonly Mock<ILogger<ReferralService>> _mockLogger;
        private readonly ReferralService _service;

        public ReferralServiceTests()
        {
            _mockReferralCodeRepository = new Mock<IReferralCodeRepository>();
            _mockReferralRepository = new Mock<IReferralRepository>();
            _mockDeepLinkService = new Mock<IDeepLinkService>();
            _mockReferralCodeGenerator = new Mock<IReferralCodeGenerator>();
            _mockLogger = new Mock<ILogger<ReferralService>>();

            _service = new ReferralService(
                _mockReferralCodeRepository.Object,
                _mockReferralRepository.Object,
                _mockDeepLinkService.Object,
                _mockReferralCodeGenerator.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetUserReferralCode_ExistingCode_ReturnsCode()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingCode = new ReferralCode()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = "ABC123",
            };

            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(existingCode);

            // Act
            var result = await _service.GetUserReferralCode(userId);

            // Assert
            Assert.Equal(existingCode.Code, result);
            _mockReferralCodeRepository.Verify(r => r.GetLatestByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserReferralCode_NewCode_GeneratesAndReturnsCode()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var generatedCode = "NEW123";
            var newReferralCode = new ReferralCode()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = generatedCode,
            };

            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(null as ReferralCode);
            _mockReferralCodeGenerator.Setup(g => g.GenerateReferralCode())
                .Returns(generatedCode);
            _mockReferralCodeRepository.Setup(r => r.GetByCodeAsync(generatedCode))
                .ReturnsAsync(null as ReferralCode);
            _mockReferralCodeRepository.Setup(r => r.AddAsync(It.IsAny<ReferralCode>()))
                .ReturnsAsync(newReferralCode);

            // Act
            var result = await _service.GetUserReferralCode(userId);

            // Assert
            Assert.Equal(generatedCode, result);
            _mockReferralCodeRepository.Verify(r => r.AddAsync(It.Is<ReferralCode>(rc =>
                rc.UserId == userId && rc.Code == generatedCode)), Times.Once);
        }

        [Fact]
        public async Task GetUserReferralCode_MaxAttemptsExceeded_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(null as ReferralCode);
            _mockReferralCodeRepository.Setup(r => r.GetByCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralCode() { Id = Guid.NewGuid(), Code = "DUPLICATE", UserId = userId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.GetUserReferralCode(userId));
            Assert.Equal("Failed to generate a unique referral code", exception.Message);
        }

        [Fact]
        public async Task GetUserReferrals_ReturnsAllReferrals()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Create first referral code
            var code1 = new ReferralCode
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = "CODE1",
                CreatedAt = DateTime.UtcNow,
                Referrals = new List<Referral>()
            };

            code1.Referrals.Add(new Referral
            {
                Id = Guid.NewGuid(),
                RefereeId = Guid.NewGuid(),
                ReferralCodeId = code1.Id,
                CreatedAt = DateTime.UtcNow
            });
            code1.Referrals.Add(new Referral
            {
                Id = Guid.NewGuid(),
                RefereeId = Guid.NewGuid(),
                ReferralCodeId = code1.Id,
                CreatedAt = DateTime.UtcNow
            });

            // Create second referral code
            var code2 = new ReferralCode
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = "CODE2",
                CreatedAt = DateTime.UtcNow,
                Referrals = new List<Referral>()
            };

            code2.Referrals.Add(new Referral
            {
                Id = Guid.NewGuid(),
                RefereeId = Guid.NewGuid(),
                ReferralCodeId = code2.Id,
                CreatedAt = DateTime.UtcNow
            });

            var referralCodes = new List<ReferralCode> { code1, code2 };

            _mockReferralCodeRepository.Setup(r => r.GetAllUserReferralsAsync(userId))
                .ReturnsAsync(referralCodes);

            // Act
            var result = await _service.GetUserReferrals(userId);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.ReferralCodeId == code1.Id);
            Assert.Contains(result, r => r.ReferralCodeId == code2.Id);
            _mockReferralCodeRepository.Verify(r => r.GetAllUserReferralsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserReferrals_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockReferralCodeRepository.Setup(r => r.GetAllUserReferralsAsync(userId))
                .ReturnsAsync(new List<ReferralCode>());

            // Act
            var result = await _service.GetUserReferrals(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GenerateReferralShortLink_Success_ReturnsDeepLink()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var referralCode = new ReferralCode()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = "ABC123",
                CreatedAt = DateTime.UtcNow,
                Referrals = new List<Referral>()
            };
            var expectedDeepLink = "https://short.link/ABC123";

            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(referralCode);
            _mockDeepLinkService.Setup(d => d.GenerateDeepLinkAsync(referralCode.Code))
                .ReturnsAsync(expectedDeepLink);

            // Act
            var result = await _service.GenerateReferralShortLink(userId);

            // Assert
            Assert.Equal(expectedDeepLink, result);
            _mockDeepLinkService.Verify(d => d.GenerateDeepLinkAsync(referralCode.Code), Times.Once);
        }

        [Fact]
        public async Task GenerateReferralShortLink_NullReferralCode_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(null as ReferralCode);
            _mockReferralCodeGenerator.Setup(g => g.GenerateReferralCode())
                .Returns("DUPLICATE");
            _mockReferralCodeRepository.Setup(r => r.GetByCodeAsync("DUPLICATE"))
                .ReturnsAsync(new ReferralCode() { Id = Guid.NewGuid(), Code = "DUPLICATE", UserId = userId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.GenerateReferralShortLink(userId));
            Assert.Equal("Failed to generate a short link", exception.Message);
        }

        [Fact]
        public async Task GenerateReferralShortLink_DeepLinkServiceFails_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var referralCode = new ReferralCode { Code = "ABC123", UserId = userId };
            _mockReferralCodeRepository.Setup(r => r.GetLatestByUserIdAsync(userId))
                .ReturnsAsync(referralCode);
            _mockDeepLinkService.Setup(d => d.GenerateDeepLinkAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Deep link service error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.GenerateReferralShortLink(userId));
            Assert.Equal("Failed to generate a short link", exception.Message);
        }

        // Note: Add these tests when implementing CreateReferral and CompleteReferral methods
        [Fact]
        public async Task CreateReferral_ThrowsNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(
                () => _service.CreateReferral("CODE", Guid.NewGuid()));
        }

        [Fact]
        public async Task CompleteReferral_ThrowsNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(
                () => _service.CompleteReferral("referralId"));
        }
    }
}