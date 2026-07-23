namespace IqasAutomationApi.DTOs;

public class DefectResponseDto
{
    public int DefectId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AiSummary { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }

    // Flattened Audit & Location fields
    public int AuditId { get; set; }
    public string AuditorName { get; set; } = string.Empty;
    public int TotalItemsAudited { get; set; }
    public DateTime AuditDate { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}