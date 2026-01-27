using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Models;
using SHSOS.Services;

namespace SHSOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class WaterController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;
        private readonly AlertService _alertService;

        public WaterController(SHSOSDbContext context, AnalyticsService analyticsService, AlertService alertService)
        {
            _context = context;
            _analyticsService = analyticsService;
            _alertService = alertService;
        }

        // API Endpoints
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet("api/water")]
        public async Task<IActionResult> GetWaterData(int? departmentId, bool? leakageOnly)
        {
            var waterData = await _context.WaterConsumption
                .FromSqlRaw("SELECT * FROM snot.vw_WaterConsumption")
                .AsNoTracking()
                .ToListAsync();

            var query = waterData.AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(w => w.DepartmentID == departmentId.Value);

            if (leakageOnly == true)
                query = query.Where(w => w.LeakageDetected);

            return Json(query.OrderByDescending(w => w.ConsumptionDate).ToList());
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("api/water")]
        public async Task<IActionResult> ApiCreate([FromBody] WaterConsumption water)
        {
            if (ModelState.IsValid)
            {
                water.RecordedAt = DateTime.Now;
                _context.Add(water);
                await _context.SaveChangesAsync();
                _alertService.CheckWaterThresholds();
                return Ok(water);
            }
            return BadRequest(ModelState);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpDelete("api/water/{id}")]
        public async Task<IActionResult> ApiDelete(int id)
        {
            var water = await _context.WaterConsumption.FindAsync(id);
            if (water == null) return NotFound();

            _context.WaterConsumption.Remove(water);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPut("api/water/{id}")]
        public async Task<IActionResult> ApiUpdate(int id, [FromBody] WaterConsumption water)
        {
            if (id != water.ConsumptionID) return BadRequest();
            if (ModelState.IsValid)
            {
                _context.Update(water);
                await _context.SaveChangesAsync();
                _alertService.CheckWaterThresholds();
                return Ok(water);
            }
            return BadRequest(ModelState);
        }

        // GET: Water
        public async Task<IActionResult> Index(int? departmentId, bool? leakageOnly)
        {
            var waterData = await _context.WaterConsumption
                .FromSqlRaw("SELECT * FROM snot.vw_WaterConsumption")
                .AsNoTracking()
                .ToListAsync();

            var query = waterData.AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(w => w.DepartmentID == departmentId.Value);

            if (leakageOnly == true)
                query = query.Where(w => w.LeakageDetected);

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.LeakageOnly = leakageOnly;

            var results = query.OrderByDescending(w => w.ConsumptionDate).ToList();

            // Summary statistics
            ViewBag.TotalLiters = results.Sum(w => w.UnitsConsumedLiters);
            ViewBag.LeakageCount = results.Count(w => w.LeakageDetected);
            ViewBag.TotalCost = results.Sum(w => w.UnitsConsumedLiters * w.UnitCost);

            return View(results);
        }

        // GET: Water/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var water = await _context.WaterConsumption
                .FromSqlRaw("SELECT * FROM snot.vw_WaterConsumption")
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ConsumptionID == id);

            if (water == null)
                return NotFound();

            return View(water);
        }

        // GET: Water/Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: Water/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,ConsumptionDate,ReadingTime,ReadingEnd,UnitsConsumedLiters,UnitCost,LeakageDetected,WeatherCategory,WeatherCondition,Remarks")] WaterConsumption water)
        {
            if (ModelState.IsValid)
            {
                water.RecordedAt = DateTime.Now;
                _context.Add(water);
                await _context.SaveChangesAsync();
                _alertService.CheckWaterThresholds();

                TempData["SuccessMessage"] = "Water consumption record created successfully!";
                
                // Create alert if leakage detected
                if (water.LeakageDetected)
                {
                    TempData["WarningMessage"] = "⚠️ Leakage detected! Please investigate immediately.";
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", water.DepartmentID);
            return View(water);
        }

        // GET: Water/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var water = await _context.WaterConsumption.FindAsync(id);
            if (water == null)
                return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", water.DepartmentID);
            return View(water);
        }

        // POST: Water/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumptionID,DepartmentID,ConsumptionDate,ReadingTime,ReadingEnd,UnitsConsumedLiters,UnitCost,LeakageDetected,WeatherCategory,WeatherCondition,Remarks,RecordedAt")] WaterConsumption water)
        {
            if (id != water.ConsumptionID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Use Stored Procedure for updates
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC snot.usp_UpdateWater @ConsumptionID, @UnitsConsumedLiters, @ConsumptionDate, @LeakageDetected, @Remarks",
                        new Microsoft.Data.SqlClient.SqlParameter("@ConsumptionID", water.ConsumptionID),
                        new Microsoft.Data.SqlClient.SqlParameter("@UnitsConsumedLiters", water.UnitsConsumedLiters),
                        new Microsoft.Data.SqlClient.SqlParameter("@ConsumptionDate", water.ConsumptionDate),
                        new Microsoft.Data.SqlClient.SqlParameter("@LeakageDetected", water.LeakageDetected),
                        new Microsoft.Data.SqlClient.SqlParameter("@Remarks", water.Remarks ?? (object)DBNull.Value)
                    );

                    _alertService.CheckWaterThresholds();
                    TempData["SuccessMessage"] = "Water consumption record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterExists(water.ConsumptionID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", water.DepartmentID);
            return View(water);
        }

        // GET: Water/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var water = await _context.WaterConsumption
                .Include(w => w.Departments)
                .FirstOrDefaultAsync(m => m.ConsumptionID == id);

            if (water == null)
                return NotFound();

            return View(water);
        }

        // POST: Water/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var water = await _context.WaterConsumption.FindAsync(id);
            if (water != null)
            {
                _context.WaterConsumption.Remove(water);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Water consumption record deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WaterExists(int id)
        {
            return _context.WaterConsumption.Any(w => w.ConsumptionID == id);
        }
    }
}
