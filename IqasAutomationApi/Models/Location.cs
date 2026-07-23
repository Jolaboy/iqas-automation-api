namespace IqasAutomationApi.Models;

public class Location
{
    public int LocationId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Audit> Audits { get; set; } = new List<Audit>();
}