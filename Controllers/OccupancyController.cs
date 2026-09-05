using Microsoft.AspNetCore.Mvc;
using XORevenueEngine.Services;

namespace XORevenueEngine.Controllers
{
    [ApiController]
    [Route("api/occupancy")]
    public class OccupancyController : ControllerBase
    {
        private readonly OccupancyService _service;

        public OccupancyController(OccupancyService service)
        {
            _service = service;
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyOccupancy()
        {
            var result = await _service.GetMonthlyOccupancy();

            return Ok(result);
        }
        [HttpGet("daily")]
public async Task<IActionResult> GetDailyOccupancy()
{
    var result = await _service.GetDailyOccupancy();

    return Ok(result);
}
[HttpGet("pickup")]
public async Task<IActionResult> GetPickupTrend()
{
    var result = await _service.GetPickupTrend();

    return Ok(result);
}
[HttpGet("alerts")]
public async Task<IActionResult> GetRevenueAlerts()
{
    var result = await _service.GetRevenueAlerts();

    return Ok(result);
}
    }
}