namespace IqasAutomationApi.Models;

public class Audit
{
    public int AuditId { get; set; }
    public int LocationId { get; set; }
    public string AuditorName { get; set; } = string.Empty;
    public int TotalItemsAudited { get; set; }
    public DateTime AuditDate { get; set; } = DateTime.UtcNow;

    public Location? Location { get; set; }
    public ICollection<Defect> Defects { get; set; } = new List<Defect>();
}