using System;
using System.Collections.Generic;
using XORevenueEngine.Models;

namespace XORevenueEngine.Services
{
    public class AlertService
    {
        public List<Alert>
            GenerateAlerts(
            dynamic dailyData
        )
        {
            var alerts =
                new List<Alert>();

            foreach (var day in dailyData)
            {
                double occupancy =
                    day.occupancy * 100;

                // Demo thresholds adjusted for sample dataset
                // LOW DEMAND ALERT
                if (occupancy < 10)
                {
                    alerts.Add(new Alert
                    {
                        AlertDate =
                            day.date,

                        AlertType =
                            "Low Demand Alert",

                        Severity = "Medium",

                        Recommendation =
                            "Push offers / promotions / OTA visibility",

                        Status =
                            "Open",

                        CreatedAt =
                            DateTime.Now
                            .ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            )
                    });
                }

                // HIGH DEMAND ALERT
                else if (
                    occupancy > 25
                )
                {
                    alerts.Add(new Alert
                    {
                        AlertDate =
                            day.date,

                        AlertType =
                            "High Demand Alert",

                        Severity =
                            "High",

                        Recommendation =
                            "Review pricing and increase room rates",

                        Status =
                            "Open",

                        CreatedAt =
                            DateTime.Now
                            .ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            )
                    });
                }

                // PICKUP SPIKE
                if (
                    day.roomsOccupied >= 30
                )
                {
                    alerts.Add(new Alert
                    {
                        AlertDate =
                            day.date,

                        AlertType =
                            "Pickup Spike Alert",

                        Severity =
                            "High",

                        Recommendation =
                            "Check event demand / group booking / sudden market demand",

                        Status =
                            "Open",

                        CreatedAt =
                            DateTime.Now
                            .ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            )
                    });
                }
            }

            return alerts;
        }
    }
}