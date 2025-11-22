using PROGI_Challenge.Api.Models;
using PROGI_Challenge.Api.Services;
using Xunit;

namespace PROGI_Challenge.Tests
{
    public sealed class BidCalculatorServiceTests
    {
        [Theory]
        [InlineData(398.00, "Common", 39.80, 7.96, 5.00, 100.00, 550.76)]
        [InlineData(501.00, "Common", 50.00, 10.02, 10.00, 100.00, 671.02)]
        [InlineData(57.00, "Common", 10.00, 1.14, 5.00, 100.00, 173.14)]
        [InlineData(1800.00, "Luxury", 180.00, 72.00, 15.00, 100.00, 2167.00)]
        [InlineData(1100.00, "Common", 50.00, 22.00, 15.00, 100.00, 1287.00)]
        [InlineData(1000000.00, "Luxury", 200.00, 40000.00, 20.00, 100.00, 1040320.00)]
        public void Calculate_UsesExpectedValuesFromSpecification(
            decimal price,
            string vehicleType,
            decimal expectedBasic,
            decimal expectedSpecial,
            decimal expectedAssociation,
            decimal expectedStorage,
            decimal expectedTotal)
        {
            BidCalculator calculator = new BidCalculator();
            BidRequestDto request = new BidRequestDto
            {
                Price = price,
                VehicleType = vehicleType
            };

            BidResponseDto result = calculator.Calculate(request);

            Assert.Equal(expectedBasic, result.BasicBuyerFee);
            Assert.Equal(expectedSpecial, result.SellerSpecialFee);
            Assert.Equal(expectedAssociation, result.AssociationFee);
            Assert.Equal(expectedStorage, result.StorageFee);
            Assert.Equal(expectedTotal, result.Total);
        }
    }
}
