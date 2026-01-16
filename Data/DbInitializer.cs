using SHSOS.Models;
using System;
using System.Linq;

namespace SHSOS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SHSOSDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any hospitals.
            if (context.hospitals.Any())
            {
                return;   // DB has been seeded
            }

            var hospitals = new hospitals[]
            {
                new hospitals { HospitalName = "City General Hospital", Location = "Downtown" },
                new hospitals { HospitalName = "Green Leaf Medical Center", Location = "Westside" }
            };
            foreach (var h in hospitals)
            {
                context.hospitals.Add(h);
            }
            context.SaveChanges();

            var departments = new Departments[]
            {
                // City General Hospital (ID likely 1)
                new Departments { HospitalID = hospitals[0].HospitalID, DepartmentName = "Cardiology", FloorNumber = 1, Inactive = false },
                new Departments { HospitalID = hospitals[0].HospitalID, DepartmentName = "Neurology", FloorNumber = 2, Inactive = false },
                // Green Leaf Medical Center (ID likely 2)
                new Departments { HospitalID = hospitals[1].HospitalID, DepartmentName = "Pediatrics", FloorNumber = 1, Inactive = false }
            };
            foreach (var d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var energy = new EnergyConsumption[]
            {
                new EnergyConsumption {
                    DepartmentID = departments[0].DepartmentID,
                    ConsumptionDate = DateTime.Parse("2026-01-15"),
                    ReadingTime = TimeSpan.Parse("08:00"),
                    MeterReadingStart = 1000,
                    UnitsConsumedkWh = 500,
                    UnitCost = 0.12m,
                    UsageCategory = "High",
                    PeakHourFlag = true,
                    TotalCost = 60.00m,
                    CarbonEmissionsKg = 250,
                    RecordedAt = DateTime.Now
                },
                new EnergyConsumption {
                    DepartmentID = departments[1].DepartmentID,
                    ConsumptionDate = DateTime.Parse("2026-01-15"),
                    ReadingTime = TimeSpan.Parse("09:00"),
                    MeterReadingStart = 2000,
                    UnitsConsumedkWh = 300,
                    UnitCost = 0.12m,
                    UsageCategory = "Normal",
                    PeakHourFlag = false,
                    TotalCost = 36.00m,
                    CarbonEmissionsKg = 150,
                    RecordedAt = DateTime.Now
                }
            };
            foreach (var e in energy)
            {
                context.EnergyConsumption.Add(e);
            }

            var water = new WaterConsumption[]
            {
                new WaterConsumption {
                    DepartmentID = departments[0].DepartmentID,
                    ConsumptionDate = DateTime.Parse("2026-01-15"),
                    ReadingTime = TimeSpan.Parse("08:30"),
                    ReadingEnd = 5000,
                    UnitsConsumedLiters = 1200,
                    UnitCost = 0.05m,
                    LeakageDetected = false,
                    WeatherCategory = "Sunny",
                    WeatherCondition = "Clear", // Added required field
                    Remarks = "Normal usage", // Added required field
                    RecordedAt = DateTime.Now
                }
            };
            foreach (var w in water)
            {
                context.WaterConsumption.Add(w);
            }

            var waste = new WasteManagement[]
            {
                new WasteManagement {
                    DepartmentID = departments[1].DepartmentID,
                    WasteType = "Biological",
                    WasteCategory = "Hazardous",
                    WasteWeight = 50.5m,
                    SegregationStatus = "Segregated",
                    DisposalMethod = "Incineration",
                    DisposalCost = 100.00m,
                    DisinfectionCost = 20.00m,
                    ComplianceStatus = "Compliant",
                    CollectionDate = DateTime.Parse("2026-01-14"),
                    RecordedAt = DateTime.Now
                }
            };
            foreach (var w in waste)
            {
                context.WasteManagement.Add(w);
            }

            context.SaveChanges();
        }
    }
}
