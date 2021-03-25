using Bogus;
using FluentAssertions;
using Library.TestHelpers;
using Processing.ScheduledWorker.Domain.Models;
using Processing.ScheduledWorker.Domain.Services;
using Xunit;

namespace Processing.ScheduledWorker.UnitTests.Domain.Services
{
    [Trait("unit-test", "Processing.ScheduledWorker-domain")]
    public class CustomerCpfComparerTests
    {
        [Fact]
        public void Sort_Should_Sort_Collection_Correctly()
        {
            // arrange
            var collectionToSort = Fakes.CPFs.Valid().Generate(3).ConvertAll(x => new Customer { Cpf = x });
            var sut = new CpfCarrierComparer();

            // act
            collectionToSort.Sort(sut);

            // assert
            collectionToSort.Should().BeInAscendingOrder(sut);
            ulong aux = 0;
            collectionToSort.TrueForAll(x =>
            {
                var isGreater = x.Cpf > aux;
                aux = x.Cpf;
                return isGreater;
            }).Should().BeTrue();
        }

        [Fact]
        public void BinarySearch_Should_Return_CollectionItem_Index_Correctly()
        {
            // arrange
            var collectionToSort = Fakes.CPFs.Valid().Generate(3).ConvertAll(x => new Customer { Cpf = x });
            var index = new Faker().Random.Int(0, collectionToSort.Count - 1);
            var sut = new CpfCarrierComparer();
            collectionToSort.Sort(sut);

            // act
            var result = collectionToSort.BinarySearch(collectionToSort[index], sut);

            // assert
            collectionToSort.Should().BeInAscendingOrder(sut);
            ulong aux = 0;
            collectionToSort.TrueForAll(x =>
            {
                var isGreater = x.Cpf > aux;
                aux = x.Cpf;
                return isGreater;
            }).Should().BeTrue();
            result.Should().Be(index);
        }

        [Fact]
        public void BinarySearch_Should_Return_NoCollectionItem_NegativeIndex()
        {
            // arrange
            var collectionToSort = Fakes.CPFs.Valid().Generate(3).ConvertAll(x => new Customer { Cpf = x });
            var notInCollection = new Customer { Cpf = Fakes.CPFs.Valid().Generate() };
            var sut = new CpfCarrierComparer();
            collectionToSort.Sort(sut);

            // act
            var result = collectionToSort.BinarySearch(notInCollection, sut);

            // assert
            collectionToSort.Should().BeInAscendingOrder(sut);
            ulong aux = 0;
            collectionToSort.TrueForAll(x =>
            {
                var isGreater = x.Cpf > aux;
                aux = x.Cpf;
                return isGreater;
            }).Should().BeTrue();
            result.Should().BeLessThan(0);
        }
    }
}
