namespace DevPairing.Api.Models;

public class UserPreferences
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool NotifyOnSlotJoin { get; set; } = true;
    public bool NotifyBeforeSessionStart { get; set; } = true;
}
