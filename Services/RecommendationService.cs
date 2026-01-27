using System;
using System.Collections.Generic;
using System.Linq;
using SHSOS.Data;
using SHSOS.Models;

namespace SHSOS.Services
{
    public class RecommendationService
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;

        public RecommendationService(SHSOSDbContext context, AnalyticsService analyticsService)
        {
            _context = context;
            _analyticsService = analyticsService;
        }

        public List<Recommendation> GetRecommendations()
        {
            var recommendations = new List<Recommendation>();

            // 1. Energy Recommendations
            var energyByDept = _analyticsService.GetEnergyByDepartment();
            if (energyByDept.Any())
            {
                var topEnergyDept = energyByDept.OrderByDescending(d => d.Value).First();
                recommendations.Add(new Recommendation
                {
                    Title = "High Energy Use Found",
                    Description = $"The {topEnergyDept.Key} team is using a lot of power ({topEnergyDept.Value:N0} kWh). Please check if lights or AC can be turned off when not needed.",
                    Category = "Energy",
                    Impact = "High"
                });
            }

            // 2. Water Recommendations
            var leakageCount = _analyticsService.GetLeakageCount(DateTime.Now.AddDays(-7));
            if (leakageCount > 0)
            {
                recommendations.Add(new Recommendation
                {
                    Title = "Fix Water Leaks Now",
                    Description = $"We found {leakageCount} leaks this week. Please call the plumber to fix them immediately to save water.",
                    Category = "Water",
                    Impact = "Critical"
                });
            }
            else
            {
                recommendations.Add(new Recommendation
                {
                    Title = "Good Job on Water!",
                    Description = "No leaks found lately. Keep up the good work and remember to turn off taps tightly.",
                    Category = "Water",
                    Impact = "Medium"
                });
            }

            // 3. Waste Recommendations
            var nonCompliantWaste = _context.WasteManagement
                .Count(w => w.ComplianceStatus != "Compliant" && w.CollectionDate >= DateTime.Now.AddDays(-30));
            
            if (nonCompliantWaste > 0)
            {
                recommendations.Add(new Recommendation
                {
                    Title = "Check Waste Sorting",
                    Description = $"Some trash was not put in the right bins {nonCompliantWaste} times this month. Please remind everyone how to sort waste correctly.",
                    Category = "Waste",
                    Impact = "High"
                });
            }

            // 4. Carbon Footprint
            var totalCarbon = _analyticsService.GetTotalCarbonEmissions(DateTime.Now.AddDays(-30));
            if (totalCarbon > 500) // Arbitrary threshold for demo
            {
                recommendations.Add(new Recommendation
                {
                    Title = "Help the Environment",
                    Description = $"Our carbon score is {totalCarbon:N0} kg Co2. Switching to solar lights outside could help lower this number.",
                    Category = "Sustainability",
                    Impact = "Medium"
                });
            }

            return recommendations;
        }
    }

    public class Recommendation
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Energy, Water, Waste, Sustainability
        public string Impact { get; set; } = string.Empty; // High, Medium, Low, Critical
    }
}
