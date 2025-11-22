using PROGI_Challenge.Api.Models;
using PROGI_Challenge.Api.Services;

namespace PROGI_Challenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class BidsController : ControllerBase
    {
        private readonly IBidCalculator _calculator;

        public BidsController(IBidCalculator calculator)
        {
            _calculator = calculator;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] BidRequestDto request)
        {
            //Validation of request not null and required fields
            if (request == null)
                return BadRequest(new ApiResponse { result = false, message = "Request cannot be null." });

            if (string.IsNullOrWhiteSpace(request.VehicleType))
                return BadRequest(new ApiResponse { result = false, message = "VehicleType is required." });

            if (request.Price <= 0)
                return BadRequest(new ApiResponse { result = false, message = "Price must be provided." });
            
            //Try to execute the calculation any error will be displayed as BadRequest/Exception
            try
            {
                var result = _calculator.Calculate(request);
                return Ok(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new ApiResponse { result = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse { result = false,message = "Internal server error." });
            }
        }
    }
}
