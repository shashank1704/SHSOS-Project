using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Services;

namespace SHSOS.Controllers
{
    public class AlertsController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AlertService _alertService;

        public AlertsController(SHSOSDbContext context, AlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        // GET: Alerts
        public IActionResult Index(string severity, bool? resolved)
        {
            var alerts = _context.Alert
                .Include(a => a.Departments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(severity))
                alerts = alerts.Where(a => a.Severity == severity);

            if (resolved.HasValue)
                alerts = alerts.Where(a => a.IsResolved == resolved.Value);
            else
                alerts = alerts.Where(a => !a.IsResolved); // Default: show only unresolved

            var alertList = alerts.OrderByDescending(a => a.CreatedAt).ToList();

            ViewBag.Severity = severity;
            ViewBag.Resolved = resolved;
            ViewBag.AlertSummary = _alertService.GetAlertSummary();

            return View(alertList);
        }

        // GET: Alerts/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var alert = _context.Alert
                .Include(a => a.Departments)
                .ThenInclude(d => d.hospitals)
                .FirstOrDefault(a => a.AlertID == id);

            if (alert == null)
                return NotFound();

            return View(alert);
        }

        // POST: Alerts/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Resolve(int id, string resolutionNotes)
        {
            _alertService.ResolveAlert(id, resolutionNotes);
            TempData["SuccessMessage"] = "Alert resolved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Manually trigger alert checks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckThresholds()
        {
            _alertService.CheckEnergyThresholds();
            _alertService.CheckWaterThresholds();
            _alertService.CheckWasteCompliance();

            TempData["SuccessMessage"] = "Threshold checks completed!";
            return RedirectToAction(nameof(Index));
        }
    }
}
