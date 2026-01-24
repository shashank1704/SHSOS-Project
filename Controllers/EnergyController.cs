using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Models;
using SHSOS.Services;

namespace SHSOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class EnergyController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;

        public EnergyController(SHSOSDbContext context, AnalyticsService analyticsService)
        {
            _context = context;
            _analyticsService = analyticsService;
        }

        // API Endpoints
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet("api/energy")]
        public async Task<IActionResult> GetEnergyData(int? departmentId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.EnergyConsumption.Include(e => e.Departments).AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentID == departmentId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ConsumptionDate <= endDate.Value);

            var energyData = await query.OrderByDescending(e => e.ConsumptionDate).ToListAsync();
            return Json(energyData);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("api/energy")]
        public async Task<IActionResult> ApiCreate([FromBody] EnergyConsumption energy)
        {
            if (ModelState.IsValid)
            {
                energy.TotalCost = energy.UnitsConsumedkWh * energy.UnitCost;
                energy.CarbonEmissionsKg = energy.UnitsConsumedkWh * 0.5m;
                energy.RecordedAt = DateTime.Now;

                _context.Add(energy);
                await _context.SaveChangesAsync();
                return Ok(energy);
            }
            return BadRequest(ModelState);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpDelete("api/energy/{id}")]
        public async Task<IActionResult> ApiDelete(int id)
        {
            var energy = await _context.EnergyConsumption.FindAsync(id);
            if (energy == null) return NotFound();

            _context.EnergyConsumption.Remove(energy);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPut("api/energy/{id}")]
        public async Task<IActionResult> ApiUpdate(int id, [FromBody] EnergyConsumption energy)
        {
            if (id != energy.EnergyConsumptionID) return BadRequest();
            if (ModelState.IsValid)
            {
                energy.TotalCost = energy.UnitsConsumedkWh * energy.UnitCost;
                energy.CarbonEmissionsKg = energy.UnitsConsumedkWh * 0.5m;
                _context.Update(energy);
                await _context.SaveChangesAsync();
                return Ok(energy);
            }
            return BadRequest(ModelState);
        }

        // GET: Energy
        public async Task<IActionResult> Index(int? departmentId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.EnergyConsumption.Include(e => e.Departments).AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentID == departmentId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ConsumptionDate <= endDate.Value);

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var energyData = await query.OrderByDescending(e => e.ConsumptionDate).ToListAsync();

            // Calculate summary statistics
            ViewBag.TotalUnits = energyData.Sum(e => e.UnitsConsumedkWh);
            ViewBag.TotalCost = energyData.Sum(e => e.TotalCost);
            ViewBag.TotalCarbon = energyData.Sum(e => e.CarbonEmissionsKg);
            ViewBag.PeakUsage = energyData.Where(e => e.PeakHourFlag).Sum(e => e.UnitsConsumedkWh);

            return View(energyData);
        }

        // GET: Energy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var energy = await _context.EnergyConsumption
                .Include(e => e.Departments)
                .ThenInclude(d => d.hospitals)
                .FirstOrDefaultAsync(m => m.EnergyConsumptionID == id);

            if (energy == null)
                return NotFound();

            // Get department's monthly average for comparison
            ViewBag.MonthlyAverage = _analyticsService.GetAverageEnergyConsumption(energy.DepartmentID, 30);

            return View(energy);
        }

        // GET: Energy/Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: Energy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,ConsumptionDate,ReadingTime,MeterReadingStart,UnitsConsumedkWh,UnitCost,UsageCategory,PeakHourFlag")] EnergyConsumption energy)
        {
            if (ModelState.IsValid)
            {
                // Calculate derived fields
                energy.TotalCost = energy.UnitsConsumedkWh * energy.UnitCost;
                energy.CarbonEmissionsKg = energy.UnitsConsumedkWh * 0.5m; // Simplified carbon calc
                energy.RecordedAt = DateTime.Now;

                _context.Add(energy);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Energy consumption record created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", energy.DepartmentID);
            return View(energy);
        }

        // GET: Energy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var energy = await _context.EnergyConsumption.FindAsync(id);
            if (energy == null)
                return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", energy.DepartmentID);
            return View(energy);
        }

        // POST: Energy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnergyConsumptionID,DepartmentID,ConsumptionDate,ReadingTime,MeterReadingStart,UnitsConsumedkWh,UnitCost,UsageCategory,PeakHourFlag,RecordedAt")] EnergyConsumption energy)
        {
            if (id != energy.EnergyConsumptionID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate derived fields
                    energy.TotalCost = energy.UnitsConsumedkWh * energy.UnitCost;
                    energy.CarbonEmissionsKg = energy.UnitsConsumedkWh * 0.5m;

                    _context.Update(energy);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Energy consumption record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnergyExists(energy.EnergyConsumptionID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", energy.DepartmentID);
            return View(energy);
        }

        // GET: Energy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var energy = await _context.EnergyConsumption
                .Include(e => e.Departments)
                .FirstOrDefaultAsync(m => m.EnergyConsumptionID == id);

            if (energy == null)
                return NotFound();

            return View(energy);
        }

        // POST: Energy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var energy = await _context.EnergyConsumption.FindAsync(id);
            if (energy != null)
            {
                _context.EnergyConsumption.Remove(energy);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Energy consumption record deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EnergyExists(int id)
        {
            return _context.EnergyConsumption.Any(e => e.EnergyConsumptionID == id);
        }
    }
}
