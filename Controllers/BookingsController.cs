using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XORevenueEngine.Data;
using XORevenueEngine.Models;
using XORevenueEngine.DTOs;

namespace XORevenueEngine.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
{
    return await _context.Bookings.ToListAsync();
}
[HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings),
                new { id = booking.Id },
                booking);
        }
        [HttpGet("{id}")]
public async Task<ActionResult<Booking>> GetBooking(int id)
{
    var booking = await _context.Bookings.FindAsync(id);

    if (booking == null)
    {
        return NotFound();
    }

    return booking;
}
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteBooking(int id)
{
    var booking = await _context.Bookings.FindAsync(id);

    if (booking == null)
    {
        return NotFound();
    }

    _context.Bookings.Remove(booking);

    await _context.SaveChangesAsync();

    return NoContent();
}
[HttpPost("upload")]
public async Task<IActionResult> UploadBookings(
    List<BookingUploadDto> bookings)
{
    var bookingEntities = bookings.Select(b => new Booking
    {
        BookingDate = DateTime.Parse(b.booking_date),

        CheckinDate = DateTime.Parse(b.checkin_date),

        Rooms = b.rooms,

        Nights = b.nights,

        Source = b.source
    }).ToList();

    await _context.Bookings.AddRangeAsync(bookingEntities);

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = $"{bookingEntities.Count} bookings uploaded successfully"
    });
}
    }
}