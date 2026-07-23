using IqasAutomationApi.Data;
using IqasAutomationApi.DTOs;
using IqasAutomationApi.Models;
using IqasAutomationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IqasAutomationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QualityDefectsController : ControllerBase
{
    private readonly IqasDbContext _context;
    private readonly QualityAiService _aiService;

    public QualityDefectsController(IqasDbContext context, QualityAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    /// <summary>
    /// Gets all defects mapped to clean, flattened DTOs.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectResponseDto>>> GetAllDefects()
    {
        var defects = await _context.Defects
            .AsNoTracking()
            .Select(d => new DefectResponseDto
            {
                DefectId = d.DefectId,
                Category = d.Category,
                Severity = d.Severity,
                Description = d.Description,
                AiSummary = d.AiSummary,
                IsResolved = d.IsResolved,
                CreatedAt = d.CreatedAt,
                AuditId = d.AuditId,
                AuditorName = d.Audit != null ? d.Audit.AuditorName : string.Empty,
                TotalItemsAudited = d.Audit != null ? d.Audit.TotalItemsAudited : 0,
                AuditDate = d.Audit != null ? d.Audit.AuditDate : DateTime.MinValue,
                BinCode = d.Audit != null && d.Audit.Location != null ? d.Audit.Location.BinCode : string.Empty,
                Zone = d.Audit != null && d.Audit.Location != null ? d.Audit.Location.Zone : string.Empty
            })
            .ToListAsync();

        return Ok(defects);
    }

    /// <summary>
    /// Log a new defect with AI root-cause analysis.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> LogDefect([FromBody] Defect defect)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (!string.IsNullOrWhiteSpace(defect.Description))
        {
            defect.AiSummary = await _aiService.GenerateRootCauseSummaryAsync(
                defect.Category,
                defect.Description
            );
        }

        _context.Defects.Add(defect);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllDefects), new { id = defect.DefectId }, defect);
    }

    /// <summary>
    /// Executes stored procedure 'sp_CalculateDefectRate' to get DPMO within a date range.
    /// </summary>
    [HttpGet("calculate-rate")]
    public async Task<ActionResult<DefectRateResultDto>> CalculateDefectRate(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var startDateParam = new SqlParameter("@StartDate", SqlDbType.DateTime2) { Value = startDate };
        var endDateParam = new SqlParameter("@EndDate", SqlDbType.DateTime2) { Value = endDate };

        var outputParam = new SqlParameter
        {
            ParameterName = "@CalculatedDefectRate",
            SqlDbType = SqlDbType.Decimal,
            Precision = 18,
            Scale = 2,
            Direction = ParameterDirection.Output
        };

        // Execute stored procedure
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CalculateDefectRate @StartDate, @EndDate, @CalculatedDefectRate OUTPUT",
            startDateParam, endDateParam, outputParam
        );

        decimal dpmoRate = outputParam.Value != DBNull.Value ? (decimal)outputParam.Value : 0.00m;

        var result = new DefectRateResultDto
        {
            StartDate = startDate,
            EndDate = endDate,
            DefectRateDpmo = dpmoRate
        };

        return Ok(result);
    }
}