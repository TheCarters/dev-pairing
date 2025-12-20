using System.ComponentModel.DataAnnotations;

namespace DevPairing.Api.Models;

public class PairingSlot
{
    public int Id { get; set; }
    
    public int DevGroupId { get; set; }
    public DevGroup DevGroup { get; set; } = null!;
    
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? NtfyTopic { get; set; }
    
    public ICollection<PairingSignup> Signups { get; set; } = new List<PairingSignup>();
}
