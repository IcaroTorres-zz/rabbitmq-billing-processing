using Customers.Application.Abstractions;
using Library.Results;
using Moq;
using System.Threading;

namespace UnitTests.Customers.Helpers
{
    public sealed class UnitofworkMockBuilder
    {
        private readonly Mock<IUnitofwork> _mock;

        private UnitofworkMockBuilder()
        {
            _mock = new Mock<IUnitofwork>();
        }

        public static UnitofworkMockBuilder Create()
        {
            return new UnitofworkMockBuilder();
        }

        public UnitofworkMockBuilder BeginTransaction(IUnitofwork unit = null)
        {
            _mock.Setup(x => x.BeginTransaction()).Returns(unit ?? _mock.Object);
            return this;
        }

        public UnitofworkMockBuilder Commit(IResult expectedResult)
        {
            _mock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            return this;
        }

        public IUnitofwork Build()
        {
            return _mock.Object;
        }
    }
}
