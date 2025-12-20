using System.ComponentModel.DataAnnotations;

namespace DevPairing.Api.Models;

public class DevGroup
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public Guid Identifier { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<UserGroupMembership> Memberships { get; set; } = new List<UserGroupMembership>();
    public ICollection<PairingSlot> Slots { get; set; } = new List<PairingSlot>();
}
