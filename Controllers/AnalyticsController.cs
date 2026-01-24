using Microsoft.AspNetCore.Mvc;
using SHSOS.Data;
using SHSOS.Services;
using Microsoft.EntityFrameworkCore;

namespace SHSOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AnalyticsController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;
        private readonly PredictionService _predictionService;
        private readonly SustainabilityService _sustainabilityService;

        public AnalyticsController(
            SHSOSDbContext context,
            AnalyticsService analyticsService,
            PredictionService predictionService,
            SustainabilityService sustainabilityService)
        {
            _context = context;
            _analyticsService = analyticsService;
            _predictionService = predictionService;
            _sustainabilityService = sustainabilityService;
        }

        public IActionResult Index()
        {
            // Trend Analysis
            ViewBag.EnergyTrend = _analyticsService.GetEnergyTrend(30);
            ViewBag.EnergyByDepartment = _analyticsService.GetEnergyByDepartment();
            ViewBag.WaterByDepartment = _analyticsService.GetWaterByDepartment();
            ViewBag.WasteByCategory = _analyticsService.GetWasteByCategory();

            // Predictions
            ViewBag.PredictedEnergy = _predictionService.PredictEnergyTrend(30, 7);
            ViewBag.PredictedCost = _predictionService.PredictTotalMonthlyCost();

            // Anomalies
            ViewBag.Anomalies = _predictionService.DetectAnomalies(30);

            // Cost Analysis
            ViewBag.TotalEnergyCost = _analyticsService.GetTotalEnergyCost();
            ViewBag.TotalWasteCost = _analyticsService.GetTotalWasteCost();
            ViewBag.TotalCarbon = _analyticsService.GetTotalCarbonEmissions();

            // Sustainability Metrics
            var departments = _context.Departments.ToList();
            var sustainabilityData = new Dictionary<string, decimal>();

            foreach (var dept in departments)
            {
                var metrics = _sustainabilityService.CalculateSustainabilityScore(dept.DepartmentID);
                sustainabilityData[dept.DepartmentName] = metrics.SustainabilityScore;
            }

            ViewBag.SustainabilityData = sustainabilityData;

            return View();
        }

        // API Endpoint for Chart Data
        [HttpGet]
        public JsonResult GetEnergyTrendData(int days = 30)
        {
            var data = _analyticsService.GetEnergyTrend(days);
            return Json(new
            {
                labels = data.Select(d => d.Date.ToString("MMM dd")).ToArray(),
                values = data.Select(d => d.Value).ToArray()
            });
        }

        [HttpGet]
        public JsonResult GetDepartmentComparison()
        {
            var energyByDept = _analyticsService.GetEnergyByDepartment();
            var waterByDept = _analyticsService.GetWaterByDepartment();

            return Json(new
            {
                departments = energyByDept.Keys.ToArray(),
                energy = energyByDept.Values.ToArray(),
                water = waterByDept.Values.ToArray()    
            });
        }

        [HttpGet]
        public JsonResult GetWasteDistribution()
        {
            var wasteByCategory = _analyticsService.GetWasteByCategory();

            return Json(new
            {
                labels = wasteByCategory.Keys.ToArray(),
                values = wasteByCategory.Values.ToArray()
            });
        }
    }
}
