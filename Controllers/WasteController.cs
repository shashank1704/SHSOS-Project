using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Models;
using SHSOS.Services;

namespace SHSOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class WasteController : Controller
    {
        private readonly SHSOSDbContext _context;
        private readonly AnalyticsService _analyticsService;
        private readonly AlertService _alertService;

        public WasteController(SHSOSDbContext context, AnalyticsService analyticsService, AlertService alertService)
        {
            _context = context;
            _analyticsService = analyticsService;
            _alertService = alertService;
        }

        // API Endpoints
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet("api/waste")]
        public async Task<IActionResult> GetWasteData(int? departmentId, string wasteCategory)
        {
            var wasteData = await _context.WasteManagement
                .FromSqlRaw("SELECT * FROM snot.vw_WasteManagement")
                .AsNoTracking()
                .ToListAsync();

            var query = wasteData.AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(w => w.DepartmentID == departmentId.Value);

            if (!string.IsNullOrEmpty(wasteCategory))
                query = query.Where(w => w.WasteCategory == wasteCategory);

            return Json(query.OrderByDescending(w => w.CollectionDate).ToList());
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("api/waste")]
        public async Task<IActionResult> ApiCreate([FromBody] WasteManagement waste)
        {
            if (ModelState.IsValid)
            {
                waste.RecordedAt = DateTime.Now;
                _context.Add(waste);
                await _context.SaveChangesAsync();
                _alertService.CheckWasteCompliance();
                return Ok(waste);
            }
            return BadRequest(ModelState);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpDelete("api/waste/{id}")]
        public async Task<IActionResult> ApiDelete(int id)
        {
            var waste = await _context.WasteManagement.FindAsync(id);
            if (waste == null) return NotFound();

            _context.WasteManagement.Remove(waste);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPut("api/waste/{id}")]
        public async Task<IActionResult> ApiUpdate(int id, [FromBody] WasteManagement waste)
        {
            if (id != waste.WasteRecordID) return BadRequest();
            if (ModelState.IsValid)
            {
                _context.Update(waste);
                await _context.SaveChangesAsync();
                _alertService.CheckWasteCompliance();
                return Ok(waste);
            }
            return BadRequest(ModelState);
        }

        // GET: Waste
        public async Task<IActionResult> Index(int? departmentId, string wasteCategory)
        {
            var wasteData = await _context.WasteManagement
                .FromSqlRaw("SELECT * FROM snot.vw_WasteManagement")
                .AsNoTracking()
                .ToListAsync();

            var query = wasteData.AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(w => w.DepartmentID == departmentId.Value);

            if (!string.IsNullOrEmpty(wasteCategory))
                query = query.Where(w => w.WasteCategory == wasteCategory);

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.WasteCategory = wasteCategory;

            var results = query.OrderByDescending(w => w.CollectionDate).ToList();

            // Summary statistics
            ViewBag.TotalWeight = results.Sum(w => w.WasteWeight);
            ViewBag.NonCompliantCount = results.Count(w => w.ComplianceStatus != "Compliant");
            ViewBag.TotalDisposalCost = results.Sum(w => w.DisposalCost + w.DisinfectionCost);
            ViewBag.WasteByCategory = _analyticsService.GetWasteByCategory();

            return View(results);
        }

        // GET: Waste/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var waste = await _context.WasteManagement
                .FromSqlRaw("SELECT * FROM snot.vw_WasteManagement")
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.WasteRecordID == id);

            if (waste == null)
                return NotFound();

            return View(waste);
        }

        // GET: Waste/Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: Waste/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,WasteType,WasteCategory,WasteWeight,SegregationStatus,DisposalMethod,DisposalCost,DisinfectionCost,ComplianceStatus,CollectionDate")] WasteManagement waste)
        {
            if (ModelState.IsValid)
            {
                waste.RecordedAt = DateTime.Now;
                _context.Add(waste);
                await _context.SaveChangesAsync();
                _alertService.CheckWasteCompliance();

                TempData["SuccessMessage"] = "Waste management record created successfully!";

                // Warning for non-compliant waste
                if (waste.ComplianceStatus != "Compliant")
                {
                    TempData["WarningMessage"] = $"⚠️ Non-compliant waste! Status: {waste.ComplianceStatus}";
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", waste.DepartmentID);
            return View(waste);
        }

        // GET: Waste/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var waste = await _context.WasteManagement.FindAsync(id);
            if (waste == null)
                return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", waste.DepartmentID);
            return View(waste);
        }

        // POST: Waste/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WasteRecordID,DepartmentID,WasteType,WasteCategory,WasteWeight,SegregationStatus,DisposalMethod,DisposalCost,DisinfectionCost,ComplianceStatus,CollectionDate,RecordedAt")] WasteManagement waste)
        {
            if (id != waste.WasteRecordID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Use Stored Procedure for updates
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC snot.usp_UpdateWaste @WasteRecordID, @WasteType, @WasteWeight, @CollectionDate, @ComplianceStatus",
                        new Microsoft.Data.SqlClient.SqlParameter("@WasteRecordID", waste.WasteRecordID),
                        new Microsoft.Data.SqlClient.SqlParameter("@WasteType", waste.WasteType ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@WasteWeight", waste.WasteWeight),
                        new Microsoft.Data.SqlClient.SqlParameter("@CollectionDate", waste.CollectionDate),
                        new Microsoft.Data.SqlClient.SqlParameter("@ComplianceStatus", waste.ComplianceStatus ?? (object)DBNull.Value)
                    );

                    _alertService.CheckWasteCompliance();
                    TempData["SuccessMessage"] = "Waste management record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WasteExists(waste.WasteRecordID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", waste.DepartmentID);
            return View(waste);
        }

        // GET: Waste/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var waste = await _context.WasteManagement
                .Include(w => w.Departments)
                .FirstOrDefaultAsync(m => m.WasteRecordID == id);

            if (waste == null)
                return NotFound();

            return View(waste);
        }

        // POST: Waste/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waste = await _context.WasteManagement.FindAsync(id);
            if (waste != null)
            {
                _context.WasteManagement.Remove(waste);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Waste management record deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WasteExists(int id)
        {
            return _context.WasteManagement.Any(w => w.WasteRecordID == id);
        }
    }
}
