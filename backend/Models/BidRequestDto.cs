namespace PROGI_Challenge.Api.Models
{
    public sealed class BidRequestDto
    {
        public decimal Price { get; set; }

        // Accepts values like "Common" or "Luxury" (case-insensitive).
        public string VehicleType { get; set; } = "Common";
    }
}
