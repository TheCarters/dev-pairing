using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Endpoints;

public static class SlotEndpoints
{
    public static void MapSlotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/slots").WithTags("Slots");

        group.MapGet("/", async (int groupId, DateTime start, DateTime end, AppDbContext db) =>
        {
            var slots = await db.PairingSlots
                .Where(s => s.DevGroupId == groupId && s.StartTime >= start && s.EndTime <= end)
                .Include(s => s.Owner)
                .Include(s => s.Signups)
                    .ThenInclude(signup => signup.User)
                .ToListAsync();

            var dtos = slots.Select(ToDto).ToList();
            return Results.Ok(dtos);
        });

        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            var slot = await db.PairingSlots
                .Include(s => s.Owner)
                .Include(s => s.Signups)
                    .ThenInclude(signup => signup.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slot == null) return Results.NotFound();
            return Results.Ok(ToDto(slot));
        });

        group.MapPost("/", async (CreatePairingSlotDto input, AppDbContext db) =>
        {
            var slot = new PairingSlot
            {
                DevGroupId = input.DevGroupId,
                OwnerId = input.OwnerId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Title = input.Title,
                Description = input.Description,
                NtfyTopic = Guid.NewGuid().ToString() // Generate a topic
            };

            db.PairingSlots.Add(slot);
            await db.SaveChangesAsync();

            // Re-fetch to get owner info if needed, or construct manually.
            // For simplicity, just return what we have, but DTO expects Owner UserDto.
            var owner = await db.Users.FindAsync(input.OwnerId);
            slot.Owner = owner!; // Assume found as FK constraint would fail otherwise (roughly)

            return Results.Created($"/api/slots/{slot.Id}", ToDto(slot));
        });

        group.MapPut("/{id}", async (int id, UpdatePairingSlotDto input, AppDbContext db) =>
        {
            var slot = await db.PairingSlots.FindAsync(id);
            if (slot == null) return Results.NotFound();

            slot.StartTime = input.StartTime;
            slot.EndTime = input.EndTime;
            slot.Title = input.Title;
            slot.Description = input.Description;

            await db.SaveChangesAsync();
            
            // Need to load relations for DTO return or just return NoContent?
            // Returning updated DTO is nice.
            await db.Entry(slot).Reference(s => s.Owner).LoadAsync();
            await db.Entry(slot).Collection(s => s.Signups).Query().Include(su => su.User).LoadAsync();

            return Results.Ok(ToDto(slot));
        });

        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var slot = await db.PairingSlots.FindAsync(id);
            if (slot == null) return Results.NotFound();

            db.PairingSlots.Remove(slot);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    private static PairingSlotDto ToDto(PairingSlot slot)
    {
        return new PairingSlotDto(
            slot.Id,
            slot.DevGroupId,
            new UserDto(slot.Owner.Id, slot.Owner.FirstName, slot.Owner.Email, slot.Owner.CreatedAt),
            slot.StartTime,
            slot.EndTime,
            slot.Title,
            slot.Description,
            slot.NtfyTopic,
            slot.Signups.Select(s => new PairingSignupDto(
                s.Id,
                s.SlotId,
                new UserDto(s.User.Id, s.User.FirstName, s.User.Email, s.User.CreatedAt),
                s.SignedUpAt,
                s.Status
            )).ToList()
        );
    }
}
