using SHSOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SHSOS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SHSOSDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.hospitals.Any())
            {
                return;   // DB has been seeded
            }

            var hospitalsList = new hospitals[]
            {
                new hospitals { HospitalName = "City General Hospital", Location = "Downtown" },
                new hospitals { HospitalName = "Green Leaf Medical Center", Location = "Westside" }
            };
            context.hospitals.AddRange(hospitalsList);
            context.SaveChanges();

            var departmentsList = new List<Departments>
            {
                new Departments { HospitalID = hospitalsList[0].HospitalID, DepartmentName = "Cardiology", FloorNumber = 1, Inactive = false },
                new Departments { HospitalID = hospitalsList[0].HospitalID, DepartmentName = "Neurology", FloorNumber = 2, Inactive = false },
                new Departments { HospitalID = hospitalsList[0].HospitalID, DepartmentName = "Oncology", FloorNumber = 3, Inactive = false },
                new Departments { HospitalID = hospitalsList[0].HospitalID, DepartmentName = "Emergency", FloorNumber = 0, Inactive = false },
                new Departments { HospitalID = hospitalsList[1].HospitalID, DepartmentName = "Pediatrics", FloorNumber = 1, Inactive = false },
                new Departments { HospitalID = hospitalsList[1].HospitalID, DepartmentName = "Radiology", FloorNumber = 1, Inactive = false }
            };
            context.Departments.AddRange(departmentsList);
            context.SaveChanges();

            var random = new Random();
            var startDate = DateTime.Today.AddYears(-3);
            var endDate = DateTime.Today;

            // Generate 3 years of data
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                foreach (var dept in departmentsList)
                {
                    // Energy Data
                    var dailyConsumption = (decimal)(random.NextDouble() * 200 + 100); // 100-300 kWh
                    var isPeak = date.Hour >= 18 && date.Hour <= 22;
                    context.EnergyConsumption.Add(new EnergyConsumption
                    {
                        DepartmentID = dept.DepartmentID,
                        ConsumptionDate = date,
                        ReadingTime = new TimeSpan(random.Next(0, 24), random.Next(0, 60), 0),
                        MeterReadingStart = 0, // Placeholder
                        UnitsConsumedkWh = dailyConsumption,
                        UnitCost = 0.12m,
                        UsageCategory = dailyConsumption > 250 ? "High" : "Normal",
                        PeakHourFlag = isPeak,
                        TotalCost = dailyConsumption * 0.12m,
                        CarbonEmissionsKg = dailyConsumption * 0.5m,
                        RecordedAt = date
                    });

                    // Water Data
                    var dailyWater = (decimal)(random.NextDouble() * 1000 + 500); // 500-1500 Liters
                    var hasLeak = random.Next(1, 100) > 95; // 5% chance of leak
                    context.WaterConsumption.Add(new WaterConsumption
                    {
                        DepartmentID = dept.DepartmentID,
                        ConsumptionDate = date,
                        ReadingTime = new TimeSpan(random.Next(0, 24), random.Next(0, 60), 0),
                        ReadingEnd = 0, // Placeholder
                        UnitsConsumedLiters = dailyWater,
                        UnitCost = 0.05m,
                        LeakageDetected = hasLeak,
                        WeatherCategory = "Normal",
                        WeatherCondition = "Cloudy",
                        Remarks = hasLeak ? "Leakage suspected" : "Normal",
                        RecordedAt = date
                    });

                    // Waste Data (Weekly approx)
                    if (random.Next(1, 10) > 7)
                    {
                        var wasteWeight = (decimal)(random.NextDouble() * 50 + 10);
                        context.WasteManagement.Add(new WasteManagement
                        {
                            DepartmentID = dept.DepartmentID,
                            WasteType = "Mixed",
                            WasteCategory = random.Next(1, 3) == 1 ? "Hazardous" : "General",
                            WasteWeight = wasteWeight,
                            SegregationStatus = "Segregated",
                            DisposalMethod = "Incineration",
                            DisposalCost = wasteWeight * 2.0m,
                            DisinfectionCost = wasteWeight * 0.5m,
                            ComplianceStatus = random.Next(1, 10) > 8 ? "Non-Compliant" : "Compliant",
                            CollectionDate = date,
                            RecordedAt = date
                        });
                    }
                }

                // Batch Save to avoid memory issues for 3 years * multiple depts
                if (date.Day == 1) 
                {
                    context.SaveChanges();
                }
            }
            context.SaveChanges();

            // Alerts
            var cardiology = departmentsList.First(d => d.DepartmentName == "Cardiology");
            var radiology = departmentsList.First(d => d.DepartmentName == "Radiology");

            var alerts = new Alert[]
            {
                new Alert { 
                    DepartmentID = cardiology.DepartmentID,
                    AlertType = "Energy", 
                    Severity = "High", 
                    Message = "Sudden spike in Cardiology energy usage.", 
                    IsResolved = false, 
                    CreatedAt = DateTime.Now 
                },
                new Alert { 
                    DepartmentID = radiology.DepartmentID,
                    AlertType = "Water", 
                    Severity = "Critical", 
                    Message = "Leak detected in Radiology - B Block.", 
                    IsResolved = false, 
                    CreatedAt = DateTime.Now 
                }
            };
            context.Alerts.AddRange(alerts);
            context.SaveChanges();
        }
    }
}
