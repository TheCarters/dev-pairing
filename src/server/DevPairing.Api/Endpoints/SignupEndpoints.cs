using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

using DevPairing.Api.Services;

namespace DevPairing.Api.Endpoints;

public static class SignupEndpoints
{
    public static void MapSignupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/signups").WithTags("Signups");

        group.MapPost("/", async (CreatePairingSignupDto input, AppDbContext db, INtfyService ntfyService) =>
        {
            // Check if already signed up
            var exists = await db.PairingSignups.AnyAsync(s => s.SlotId == input.SlotId && s.UserId == input.UserId);
            if (exists) return Results.Conflict("User already signed up for this slot");

            var signup = new PairingSignup
            {
                SlotId = input.SlotId,
                UserId = input.UserId,
                SignedUpAt = DateTime.UtcNow,
                Status = "Confirmed"
            };

            db.PairingSignups.Add(signup);
            await db.SaveChangesAsync();

            // Load relations
            await db.Entry(signup).Reference(s => s.User).LoadAsync();
            await db.Entry(signup).Reference(s => s.Slot).Query()
                .Include(s => s.Owner)
                .ThenInclude(o => o.Preferences)
                .LoadAsync();
            
            // Send Notification if Owner wants it
            var owner = signup.Slot.Owner;
            // Preferences might be null if never set/accessed, default logic:
            // If we assume default is TRUE, we can check if Preferences is null OR Preferences.NotifyOnSlotJoin is true.
            // However, UserPreferences model has default = true. 
            // Better to load or create if missing? Or just treat null as true?
            // Let's treat null as true or default.
            
            var shouldNotify = owner.Preferences?.NotifyOnSlotJoin ?? true;

            if (shouldNotify) 
            {
                await ntfyService.SendNotificationAsync(
                    $"dev-pairing-user-{owner.Id}", 
                    $"New Signup: {signup.User.FirstName}", 
                    $"{signup.User.FirstName} joined {signup.Slot.Title}"
                );
            }

            return Results.Created($"/api/signups/{signup.Id}", new PairingSignupDto(
                signup.Id,
                signup.SlotId,
                new UserDto(signup.User.Id, signup.User.FirstName, signup.User.Email, signup.User.CreatedAt),
                signup.SignedUpAt,
                signup.Status
            ));
        });

        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var signup = await db.PairingSignups.FindAsync(id);
            if (signup == null) return Results.NotFound();

            db.PairingSignups.Remove(signup);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapGet("/user/{userId}", async (int userId, AppDbContext db) =>
        {
            var signups = await db.PairingSignups
                .Where(s => s.UserId == userId)
                .Include(s => s.Slot)
                    .ThenInclude(slot => slot.Owner)
                .Include(s => s.User)
                .ToListAsync();

            return Results.Ok(signups.Select(s => new PairingSignupWithSlotDto(
                s.Id,
                new PairingSlotSummaryDto(
                    s.Slot.Id,
                    s.Slot.DevGroupId,
                    new UserDto(s.Slot.Owner.Id, s.Slot.Owner.FirstName, s.Slot.Owner.Email, s.Slot.Owner.CreatedAt),
                    s.Slot.StartTime,
                    s.Slot.EndTime,
                    s.Slot.Title,
                    s.Slot.Description,
                    s.Slot.NtfyTopic
                ),
                new UserDto(s.User.Id, s.User.FirstName, s.User.Email, s.User.CreatedAt),
                s.SignedUpAt,
                s.Status
            )));
        });
    }
}
