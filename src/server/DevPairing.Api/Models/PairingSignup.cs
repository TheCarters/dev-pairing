using System.ComponentModel.DataAnnotations;

namespace DevPairing.Api.Models;

public class PairingSignup
{
    public int Id { get; set; }
    
    public int SlotId { get; set; }
    public PairingSlot Slot { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime SignedUpAt { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Confirmed";
}
