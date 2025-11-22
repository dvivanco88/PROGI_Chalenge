using PROGI_Challenge.Api.Models;

namespace PROGI_Challenge.Api.Services
{
    // Contract for the service responsible for all bid calculations.
    public interface IBidCalculator
    {
        BidResponseDto Calculate(BidRequestDto request);
    }
}
