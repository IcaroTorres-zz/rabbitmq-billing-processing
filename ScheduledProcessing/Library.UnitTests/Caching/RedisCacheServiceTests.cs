using Bogus;
using FluentAssertions;
using Library.Caching;
using Moq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Library.UnitTests.Caching
{
    [Trait("unit-test","library")]
    public class RedisCacheServiceTests
    {

        [Fact]
        public async Task GetAsync_Should_Return_ExpectedValue_When_Key_Exists_And_NotExpired()
        {
            // arrange
            var expectedKey = "sample";
            var expectedValue = new Faker().Lorem.Paragraph(2);

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(x => x.StringGetAsync(expectedKey, default)).ReturnsAsync(expectedValue);
            var connectionMock = new Mock<IConnectionMultiplexer>();
            connectionMock.Setup(x => x.GetDatabase(It.IsAny<int>(), default)).Returns(databaseMock.Object);
            var sut = new RedisCacheService(connectionMock.Object);

            // act
            var result = await sut.GetAsync(expectedKey);

            // assert
            result.Should().NotBeNullOrEmpty().And.Be(expectedValue);
        }

        [Fact]
        public async Task SetAsync_Should_Return_True()
        {
            // arrange
            var expectedKey = "sample";
            var expectedValue = new Faker().Lorem.Paragraph(2);
            var expectedExpiry = 10;
            var expectedExpiryTimeSpan = TimeSpan.FromSeconds(expectedExpiry);
            var expectedBool = true;

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(x => x.StringSetAsync(
                expectedKey, expectedValue, expectedExpiryTimeSpan, default, default))
                .ReturnsAsync(expectedBool);
            var connectionMock = new Mock<IConnectionMultiplexer>();
            connectionMock.Setup(x => x.GetDatabase(It.IsAny<int>(), default)).Returns(databaseMock.Object);
            var sut = new RedisCacheService(connectionMock.Object);

            // act
            var result = await sut.SetAsync(expectedKey, expectedValue, expectedExpiry);

            // assert
            result.Should().Be(expectedBool);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Key_NotExists_Or_Expired()
        {
            // arrange
            var expectedKey = "sample";
            var expectedValue = (string)null;

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(x => x.StringGetAsync(expectedKey, default)).ReturnsAsync(expectedValue);
            var connectionMock = new Mock<IConnectionMultiplexer>();
            connectionMock.Setup(x => x.GetDatabase(It.IsAny<int>(), default)).Returns(databaseMock.Object);
            var sut = new RedisCacheService(connectionMock.Object);

            // act
            var result = await sut.GetAsync(expectedKey);

            // assert
            result.Should().Be(expectedValue);
        }
    }
}
