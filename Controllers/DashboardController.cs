using Microsoft.AspNetCore.Mvc;
using SHSOS.Data;
using SHSOS.Services;
using Microsoft.EntityFrameworkCore;

namespace SHSOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;
        private readonly PredictionService _predictionService;
        private readonly AlertService _alertService;
        private readonly SustainabilityService _sustainabilityService;

        public DashboardController(
            SHSOSDbContext context,
            AnalyticsService analyticsService,
            PredictionService predictionService,
            AlertService alertService,
            SustainabilityService sustainabilityService)
        {
            _context = context;
            _analyticsService = analyticsService;
            _predictionService = predictionService;
            _alertService = alertService;
            _sustainabilityService = sustainabilityService;
        }

        [HttpGet("api/dashboard/data")]
        public IActionResult GetData()
        {
            try 
            {
                var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var data = new
                {
                    TotalEnergy = _analyticsService.GetTotalEnergyConsumption(),
                    TotalWater = _analyticsService.GetTotalWaterConsumption(),
                    TotalWaste = _analyticsService.GetTotalWasteGenerated(),
                    TotalCarbon = _analyticsService.GetTotalCarbonEmissions(),
                    PredictedMonthlyCost = Math.Round((decimal)_predictionService.PredictTotalMonthlyCost(), 2),
                    ActiveAlerts = _alertService.GetActiveAlerts().Take(5).ToList(),
                    SustainabilityScores = _context.Departments.ToList().Select(d => new 
                    { 
                        Name = d.DepartmentName, 
                        Score = Math.Round(_sustainabilityService.CalculateSustainabilityScore(d.DepartmentID).SustainabilityScore, 1)
                    }).ToList(),
                    EnergyTrendData = _analyticsService.GetEnergyTrend(30).Select(d => new { d.Date, Consumption = d.Value }),
                    WaterTrendData = _analyticsService.GetWaterTrend(30).Select(d => new { d.Date, Consumption = d.Value }),
                    WasteTrendData = _analyticsService.GetWasteTrend(30).Select(d => new { d.Date, Weight = d.Value })
                };

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("api/departments")]
        public IActionResult GetDepartments()
        {
            var depts = _context.Departments.Select(d => new { d.DepartmentID, d.DepartmentName }).ToList();
            return Json(depts);
        }


        public IActionResult Index()
        {
            // ===== Current Month Data =====
            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Energy Analytics
            ViewBag.TotalEnergy = _analyticsService.GetTotalEnergyConsumption();
            ViewBag.TotalEnergyCost = _analyticsService.GetTotalEnergyCost();
            ViewBag.MonthlyEnergy = _analyticsService.GetTotalEnergyConsumption(currentMonthStart);
            ViewBag.PeakEnergy = _analyticsService.GetPeakEnergyUsage();
            ViewBag.EnergyByDept = _analyticsService.GetEnergyByDepartment();

            // Water Analytics
            ViewBag.TotalWater = _analyticsService.GetTotalWaterConsumption();
            ViewBag.LeakageCount = _analyticsService.GetLeakageCount();
            ViewBag.MonthlyLeakages = _analyticsService.GetLeakageCount(currentMonthStart);
            ViewBag.WaterByDept = _analyticsService.GetWaterByDepartment();

            // Waste Analytics
            ViewBag.TotalWaste = _analyticsService.GetTotalWasteGenerated();
            ViewBag.NonCompliantWaste = _analyticsService.GetNonCompliantWasteCount();
            ViewBag.WasteByCategory = _analyticsService.GetWasteByCategory();
            ViewBag.WasteCost = _analyticsService.GetTotalWasteCost();

            // Carbon Emissions
            ViewBag.TotalCarbon = _analyticsService.GetTotalCarbonEmissions();
            ViewBag.MonthlyCarbon = _analyticsService.GetTotalCarbonEmissions(currentMonthStart);

            // Predictions
            ViewBag.PredictedMonthlyCost = _predictionService.PredictTotalMonthlyCost();
            ViewBag.EnergyTrend = _predictionService.PredictEnergyTrend(30, 7);
            ViewBag.Anomalies = _predictionService.DetectAnomalies(7);

            // Alerts
            ViewBag.ActiveAlerts = _alertService.GetActiveAlerts().Take(5).ToList();
            ViewBag.AlertSummary = _alertService.GetAlertSummary();

            // Sustainability Scores
            var departments = _context.Departments.ToList();
            var sustainabilityScores = new Dictionary<string, decimal>();
            
            foreach (var dept in departments)
            {
                var metrics = _sustainabilityService.CalculateSustainabilityScore(dept.DepartmentID);
                sustainabilityScores[dept.DepartmentName] = metrics.SustainabilityScore;
            }
            
            ViewBag.SustainabilityScores = sustainabilityScores;
            ViewBag.OverallSustainability = sustainabilityScores.Any() ? 
                Math.Round(sustainabilityScores.Values.Average(), 2) : 0;

            // Trend Data for Charts (Last 30 days)
            ViewBag.EnergyTrendData = _analyticsService.GetEnergyTrend(30);

            return View();
        }
    }
}

