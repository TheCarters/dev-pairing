namespace DevPairing.Api.Models;

public class UserGroupMembership
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int DevGroupId { get; set; }
    public DevGroup DevGroup { get; set; } = null!;
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
