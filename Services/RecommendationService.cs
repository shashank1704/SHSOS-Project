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
                    Title = "Optimize Higher Energy Zones",
                    Description = $"{topEnergyDept.Key} is consuming the most energy ({topEnergyDept.Value:N0} kWh). Consider auditing HVAC and lighting schedules in this area.",
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
                    Title = "Immediate Leak Repair Required",
                    Description = $"Detected {leakageCount} leakages in the last 7 days. Water loss is significant; prioritize plumbing maintenance.",
                    Category = "Water",
                    Impact = "Critical"
                });
            }
            else
            {
                recommendations.Add(new Recommendation
                {
                    Title = "Water Efficiency",
                    Description = "No leaks detected recently. Consider installing low-flow sensors to further optimize consumption.",
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
                    Title = "Improve Waste Compliance",
                    Description = $"There were {nonCompliantWaste} non-compliant waste instances this month. Schedule staff training on hazardous waste segregation.",
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
                    Title = "Carbon Reduction Opportunity",
                    Description = $"Monthly emissions reached {totalCarbon:N0} kg Co2. Integrating solar-powered outdoor lighting can reduce this by 15%.",
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
