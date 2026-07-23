namespace IqasAutomationApi.Models;

public class Defect
{
    public int DefectId { get; set; }
    public int AuditId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string? Description { get; set; }
    public string? AiSummary { get; set; }
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Audit? Audit { get; set; }
}