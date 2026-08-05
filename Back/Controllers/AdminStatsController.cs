using DefNotEbay_API.DTOs.AdminStats;
using DefNotEbay_API.Services;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/admin/stats")]
    [Authorize (Roles = "Admin")]
    public class AdminStatsController : ControllerBase
    {
        private readonly IAdminStatsService _statsService;
        private readonly IExportService _exportService;


        public AdminStatsController(IAdminStatsService statsService, IExportService exportService)
        {
            _statsService = statsService;
            _exportService = exportService;

        }

        [HttpGet]
        public async Task<ActionResult<AdminStatsResponse>> Get()
        {
            var result = await _statsService.GetAdminStatsAsync();
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] string? format = "json",
            [FromQuery] int? sellerId = null,
            [FromQuery] DateTime? start = null,
            [FromQuery] DateTime? end = null)
        {
            var result = await _exportService.ExportAsync(format ?? "json", sellerId, start, end);
            return File(result.Content, result.ContentType, result.FileName);
        }

    }

}
