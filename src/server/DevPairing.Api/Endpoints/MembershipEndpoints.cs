using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Endpoints;

public static class MembershipEndpoints
{
    public static void MapMembershipEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/memberships").WithTags("Memberships");

        group.MapPost("/", async (JoinGroupDto input, AppDbContext db) =>
        {
            var devGroup = await db.DevGroups.FirstOrDefaultAsync(g => g.Identifier == input.GroupIdentifier);
            if (devGroup == null) return Results.NotFound("Group not found");

            var user = await db.Users.FindAsync(input.UserId);
            if (user == null) return Results.NotFound("User not found");

            var exists = await db.Memberships.AnyAsync(m => m.UserId == input.UserId && m.DevGroupId == devGroup.Id);
            if (exists) return Results.Ok();

            db.Memberships.Add(new UserGroupMembership
            {
                UserId = input.UserId,
                DevGroupId = devGroup.Id,
                JoinedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        group.MapDelete("/{userId}/{groupId}", async (int userId, int groupId, AppDbContext db) =>
        {
            var membership = await db.Memberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.DevGroupId == groupId);

            if (membership == null) return Results.NotFound();

            db.Memberships.Remove(membership);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
