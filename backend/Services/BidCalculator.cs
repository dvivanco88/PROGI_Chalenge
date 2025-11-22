using PROGI_Challenge.Api.Models;

namespace PROGI_Challenge.Api.Services
{
    // Service that encapsulates all the business rules for fee calculation.
    public sealed class BidCalculator : IBidCalculator
    {   // (decimal literal: 'm' for more precision)
        // 10% basic buyer fee 
        private const decimal BasicBuyerFeeRate = 0.10m;

        // Basic fee bounds for common vehicles
        private const decimal CommonBasicMin = 10m;
        private const decimal CommonBasicMax = 50m;

        // Basic fee bounds for luxury vehicles
        private const decimal LuxuryBasicMin = 25m;
        private const decimal LuxuryBasicMax = 200m;

        // Special seller fee rates (decimal used to avoid float rounding issues)
        private const decimal CommonSpecialRate = 0.02m;
        private const decimal LuxurySpecialRate = 0.04m;

        // Fixed storage cost
        private const decimal StorageFeeValue = 100m;

        public BidResponseDto Calculate(BidRequestDto request)
        {
            //request NOT null guaranteed by controller

            //Price must be more than zero but in rules I dont see nothing about cents (less than $1 but more than $0) so thats why the mimum is 1
            if (request.Price < 1) 
                throw new ArgumentOutOfRangeException(nameof(request.Price), $"Price (${request.Price}) must be equal or greater than 1.");
            
            //Vehicle type is a string in request and  internally in code will try to pase to enum
            string rawVType = request.VehicleType?.Trim() ?? string.Empty;
            if (!Enum.TryParse<VehicleType>(rawVType, true, out VehicleType vehicleType)) 
                throw new ArgumentOutOfRangeException(nameof(request.VehicleType), $"VehicleType {rawVType} not recognized.");            
            

            //START CALCULATION
            decimal price = MoneyRound(request.Price);
            decimal basicFee = CalculateBasicBuyerFee(price, vehicleType);
            decimal specialFee = CalculateSellerSpecialFee(price, vehicleType);
            decimal associationFee = CalculateAssociationFee(price);
            decimal storageFee = StorageFeeValue;
            decimal total = price + basicFee + specialFee + associationFee + storageFee;
            decimal totalRounded = MoneyRound(total);
            

            //map variables to response
            BidResponseDto response = new BidResponseDto
            {
                Price = price,
                BasicBuyerFee = basicFee,
                SellerSpecialFee = specialFee,
                AssociationFee = associationFee,
                StorageFee = storageFee,
                Total = totalRounded,
                result = true,
                message = "OK"
            };
            //END CALCULATION



            return response;
        }

        // Basic buyer fee: 10% with min/max per vehicle type.
        private static decimal CalculateBasicBuyerFee(decimal price, VehicleType vehicleType)
        {
            decimal rawFee = price * BasicBuyerFeeRate;
            decimal roundedFee = MoneyRound(rawFee);

            decimal min;
            decimal max;

            //set min and max fee in $ according the Vehicle Type
            if (vehicleType == VehicleType.Common)
            {
                min = CommonBasicMin;
                max = CommonBasicMax;
            }
            else
            {
                min = LuxuryBasicMin;
                max = LuxuryBasicMax;
            }

            decimal boundedValue = ApplyBounds(roundedFee, min, max);
            return MoneyRound(boundedValue);
        }

        // Seller special fee: 2% for common, 4% for luxury.
        private static decimal CalculateSellerSpecialFee(decimal price, VehicleType vehicleType)
        {
            decimal rate = vehicleType == VehicleType.Common
                ? CommonSpecialRate
                : LuxurySpecialRate;

            decimal fee = price * rate;
            return MoneyRound(fee);
        }

        // Association fee based on price ranges from the specification.
        private static decimal CalculateAssociationFee(decimal price)
        {
            if (price <= 500m) return 5m;    
            if (price <= 1000m) return 10m;            
            if (price <= 3000m) return 15m;            
            return 20m;
        }

        // Apply Bounds value between min and max.
        private static decimal ApplyBounds(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        // Centralized rounding to two decimal places.
        private static decimal MoneyRound(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
