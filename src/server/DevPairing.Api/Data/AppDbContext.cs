using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DevGroup> DevGroups { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGroupMembership> Memberships { get; set; }
    public DbSet<PairingSlot> PairingSlots { get; set; }
    public DbSet<PairingSignup> PairingSignups { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User <-> DevGroup (Many-to-Many via Membership)
        modelBuilder.Entity<UserGroupMembership>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<UserGroupMembership>()
            .HasOne(m => m.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<UserGroupMembership>()
            .HasOne(m => m.DevGroup)
            .WithMany(g => g.Memberships)
            .HasForeignKey(m => m.DevGroupId);

        // User <-> Preferences (1-to-1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Preferences)
            .WithOne(p => p.User)
            .HasForeignKey<UserPreferences>(p => p.UserId);

        // Slot <-> Signups (1-to-Many)
        modelBuilder.Entity<PairingSignup>()
            .HasOne(s => s.Slot)
            .WithMany(p => p.Signups)
            .HasForeignKey(s => s.SlotId);
            
        modelBuilder.Entity<PairingSignup>()
            .HasOne(s => s.User)
            .WithMany(u => u.Signups)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete of user deleting history? Or Slot? 
            // Actually, if a user is deleted, signups should probably go, but we might want to keep history. 
            // But usually Cascade is fine for simple apps. 
            // I'll leave default (Cascade) for Slot->Signup, but User->Signup might need care.
            // Let's stick to default unless issues arise. Restrict on User helps prevent accidental User deletion affecting others' slots history if logic isn't clean.
            // But "User" deletion isn't a main feature yet.

        // Slot <-> Owner (User)
        modelBuilder.Entity<PairingSlot>()
            .HasOne(s => s.Owner)
            .WithMany() // No navigation property on User for "OwnedSlots" explicitly defined in Model earlier, I can add it or just leave it unidirectional from Slot.
            // Wait, User.cs didn't have OwnedSlots. I can leave it empty here or add it.
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Important: Deleting a user shouldn't just wipe out slots maybe? Or should it?
            // If Owner leaves, slot is maybe invalid.
    }
}
