namespace PROGI_Challenge.Api.Models
{
    // DTO used to return all calculated fees and the final total.
    public sealed class BidResponseDto : ApiResponse
    {
        public decimal Price { get; set; }

        public decimal BasicBuyerFee { get; set; }

        public decimal SellerSpecialFee { get; set; }

        public decimal AssociationFee { get; set; }

        public decimal StorageFee { get; set; }

        public decimal Total { get; set; }

    }
}
