using XORevenueEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace XORevenueEngine.Services
{
    public class OccupancyService
    {
        private readonly AppDbContext _context;

        private const int TOTAL_ROOMS = 100;

        public OccupancyService(AppDbContext context)
        {
            _context = context;
        }

        // ======================
        // MONTHLY OCCUPANCY
        // ======================

        public async Task<List<object>>
        GetMonthlyOccupancy()
        {
            var bookings =
                await _context.Bookings.ToListAsync();

            var monthlyOccupancy =
                bookings
                .SelectMany(b =>
                    Enumerable.Range(0, b.Nights)
                    .Select(i => new
                    {
                        Date =
                            b.CheckinDate
                            .AddDays(i),

                        Rooms = b.Rooms
                    }))
                .GroupBy(x =>
                    x.Date.ToString("yyyy-MM"))
                .Select(g =>
                {
                    var monthDate =
                        DateTime.Parse(
                            g.Key + "-01"
                        );

                    var daysInMonth =
                        DateTime.DaysInMonth(
                            monthDate.Year,
                            monthDate.Month
                        );

                    var occupiedRoomNights =
                        g.Sum(x => x.Rooms);

                    var totalAvailable =
                        TOTAL_ROOMS *
                        daysInMonth;

                    return new
                    {
                        month = g.Key,

                        occupancy =
                            (double)
                            occupiedRoomNights
                            / totalAvailable
                    };
                })
                .OrderBy(x => x.month)
                .ToList<object>();

            return monthlyOccupancy;
        }

        // ======================
        // DAILY OCCUPANCY
        // ======================

        public async Task<List<object>>
        GetDailyOccupancy()
        {
            var bookings =
                await _context
                    .Bookings
                    .ToListAsync();

            var dailyOccupancy =
                new Dictionary<
                    DateTime,
                    int
                >();

            // Expand bookings into occupied dates
            foreach (
                var booking
                in bookings
            )
            {
                for (
                    int i = 0;
                    i < booking.Nights;
                    i++
                )
                {
                    var occupiedDate =
                        booking
                        .CheckinDate
                        .Date
                        .AddDays(i);

                    if (
                        !dailyOccupancy
                        .ContainsKey(
                            occupiedDate
                        )
                    )
                    {
                        dailyOccupancy[
                            occupiedDate
                        ] = 0;
                    }

                    dailyOccupancy[
                        occupiedDate
                    ] += booking.Rooms;
                }
            }

            var result =
                dailyOccupancy
                .OrderBy(d => d.Key)
                .Select(d => new
                {
                    date =
                        d.Key.ToString(
                            "yyyy-MM-dd"
                        ),

                    roomsOccupied =
                        d.Value,

                    occupancy =
                        (double)d.Value
                        / TOTAL_ROOMS
                })
                .ToList<object>();

            return result;
        }

        // ======================
        // PICKUP TREND
        // ======================

        public async Task<List<object>>
        GetPickupTrend()
        {
            var bookings =
                await _context
                    .Bookings
                    .ToListAsync();

            int cumulative = 0;

            var pickup =
                bookings
                .OrderBy(b =>
                    b.BookingDate
                )
                .GroupBy(b =>
                    b.BookingDate.Date
                )
                .Select(g =>
                {
                    cumulative +=
                        g.Sum(x =>
                            x.Rooms);

                    return new
                    {
                        bookingDate =
                            g.Key.ToString(
                                "yyyy-MM-dd"
                            ),

                        cumulativeRoomNights =
                            cumulative
                    };
                })
                .ToList<object>();

            return pickup;
        }

        // ======================
        // REVENUE ALERTS
        // ======================

        public async Task<List<object>>
        GetRevenueAlerts()
        {
            var bookings =
                await _context
                    .Bookings
                    .ToListAsync();

            var alerts =
                new List<object>();

            var dailyOccupancy =
                new Dictionary<
                    DateTime,
                    int
                >();

            foreach (
                var booking
                in bookings
            )
            {
                for (
                    int i = 0;
                    i < booking.Nights;
                    i++
                )
                {
                    var occupiedDate =
                        booking
                        .CheckinDate
                        .Date
                        .AddDays(i);

                    if (
                        !dailyOccupancy
                        .ContainsKey(
                            occupiedDate
                        )
                    )
                    {
                        dailyOccupancy[
                            occupiedDate
                        ] = 0;
                    }

                    dailyOccupancy[
                        occupiedDate
                    ] += booking.Rooms;
                }
            }

            var monthly =
                dailyOccupancy
                .GroupBy(d =>
                    d.Key.ToString(
                        "yyyy-MM"
                    ))
                .Select(g =>
                {
                    var firstDay =
                        DateTime.Parse(
                            g.Key + "-01"
                        );

                    var daysInMonth =
                        DateTime
                        .DaysInMonth(
                            firstDay.Year,
                            firstDay.Month
                        );

                    var totalRoomNights =
                        g.Sum(x =>
                            x.Value);

                    var occupancy =
                        (
                            (double)
                            totalRoomNights
                            /
                            (
                                TOTAL_ROOMS
                                *
                                daysInMonth
                            )
                        ) * 100;

                    return new
                    {
                        month =
                            g.Key,

                        occupancy
                    };
                })
                .ToList();

            // Low & High occupancy alerts
            foreach (
                var month
                in monthly
            )
            {
                if (
                    month
                    .occupancy < 40
                )
                {
                    alerts.Add(
                        new
                        {
                            type =
                                "Low Occupancy",

                            message =
                                $"{month.month} occupancy below 40%"
                        }
                    );
                }

                if (
                    month
                    .occupancy > 80
                )
                {
                    alerts.Add(
                        new
                        {
                            type =
                                "High Occupancy",

                            message =
                                $"{month.month} occupancy above 80%"
                        }
                    );
                }
            }

            // Pickup spike alerts
            var pickup =
                bookings
                .GroupBy(b =>
                    b.BookingDate
                    .Date
                )
                .Select(g => new
                {
                    date =
                        g.Key,

                    roomsBooked =
                        g.Sum(x =>
                            x.Rooms)
                });

            foreach (
                var p
                in pickup
            )
            {
                if (
                    p.roomsBooked > 20
                )
                {
                    alerts.Add(
                        new
                        {
                            type =
                                "Booking Spike",

                            message =
                                $"High pickup detected on {p.date:yyyy-MM-dd}"
                        }
                    );
                }
            }

            return alerts;
        }
    }
}