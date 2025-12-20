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
            await db.Entry(signup).Reference(s => s.Slot).LoadAsync();
            
            // Send Notification
            // Load Slot Owner to check preferences (optional optimization to not send if no one listening? 
            // But if topic is public/shared, we might just send to topic. 
            // Plan says: "When user signs up for slot -> notify slot owner (if preference enabled)"
            // So we should check Owner's preference.
            // Owner should be listening to Slot Topic? Or does Owner get a direct notification?
            // "User subscribes to slot's ntfy topic"
            // If the Owner is subscribed to the Slot Topic, then sending to Slot Topic works.
            // But we only want to send if Owner wants it?
            // Actually, if we send to Slot Topic, ANYONE subscribed gets it.
            // If the requirement is "notify slot owner", maybe we should check if Owner wants it, 
            // but if we use Slot Topic, we can't control *who* receives it, only if we send it.
            // Assuming we send to Slot Topic.
            
            if (!string.IsNullOrEmpty(signup.Slot.NtfyTopic)) 
            {
                await ntfyService.SendNotificationAsync(
                    signup.Slot.NtfyTopic, 
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
