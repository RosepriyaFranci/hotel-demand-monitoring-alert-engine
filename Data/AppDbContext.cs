using Microsoft.EntityFrameworkCore;
using XORevenueEngine.Models;

namespace XORevenueEngine.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Alert>
        Alerts
        { get; set; }
    }
}