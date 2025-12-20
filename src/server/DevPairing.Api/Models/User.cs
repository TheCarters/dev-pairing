using System.ComponentModel.DataAnnotations;

namespace DevPairing.Api.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<UserGroupMembership> Memberships { get; set; } = new List<UserGroupMembership>();
    public ICollection<PairingSignup> Signups { get; set; } = new List<PairingSignup>();
    
    public UserPreferences? Preferences { get; set; }
}
