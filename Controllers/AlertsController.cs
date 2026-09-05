using Microsoft.AspNetCore.Mvc;
using XORevenueEngine.Models;
using XORevenueEngine.Services;
using XORevenueEngine.Data;

namespace XORevenueEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly AlertService
            _alertService;

        private readonly
            OccupancyService
            _occupancyService;

        private readonly
            AppDbContext
            _context;

        public AlertsController(
            AlertService alertService,
            OccupancyService occupancyService,
            AppDbContext context
        )
        {
            _alertService =
                alertService;

            _occupancyService =
                occupancyService;

            _context =
                context;
        }

        [HttpGet]
public async Task<IActionResult> GetAlerts()
{
    // If alerts already exist in DB
    if (_context.Alerts.Any())
    {
        return Ok(
            _context.Alerts
            .OrderBy(a => a.AlertDate)
            .ToList()
        );
    }

    // First time only → generate alerts
    var dailyData =
        await _occupancyService
        .GetDailyOccupancy();

    var alerts =
        _alertService
        .GenerateAlerts(dailyData);

    // Save generated alerts
    _context.Alerts.AddRange(alerts);

    _context.SaveChanges();

    return Ok(alerts);
}

        [HttpPut("{id}/status")]
        public IActionResult
            UpdateStatus(
                int id,
                [FromBody]
                string status
            )
        {
            var alert =
                _context.Alerts
                .FirstOrDefault(
                    a => a.Id == id
                );

            if (alert == null)
            {
                return NotFound();
            }

            alert.Status =
                status;

            _context.SaveChanges();

            return Ok(alert);
        }
    }
}