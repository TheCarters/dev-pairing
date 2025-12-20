using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/", async (CreateUserDto input, AppDbContext db) =>
        {
            // Check if user exists by email
            var user = await db.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Email == input.Email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = input.FirstName,
                    Email = input.Email,
                    CreatedAt = DateTime.UtcNow,
                    Preferences = new UserPreferences() // Default preferences
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            // Handle joining group if JoinGroupId (Identifier) provided
            if (!string.IsNullOrEmpty(input.JoinGroupId) && Guid.TryParse(input.JoinGroupId, out var groupGuid))
            {
                var devGroup = await db.DevGroups.FirstOrDefaultAsync(g => g.Identifier == groupGuid);
                if (devGroup != null)
                {
                    var isMember = await db.Memberships.AnyAsync(m => m.UserId == user.Id && m.DevGroupId == devGroup.Id);
                    if (!isMember)
                    {
                        db.Memberships.Add(new UserGroupMembership
                        {
                            UserId = user.Id,
                            DevGroupId = devGroup.Id,
                            JoinedAt = DateTime.UtcNow
                        });
                        await db.SaveChangesAsync();
                    }
                }
            }

            return Results.Ok(new UserDto(user.Id, user.FirstName, user.Email, user.CreatedAt));
        });

        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null) return Results.NotFound();
            return Results.Ok(new UserDto(user.Id, user.FirstName, user.Email, user.CreatedAt));
        });

        group.MapGet("/email/{email}", async (string email, AppDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return Results.NotFound();
            return Results.Ok(new UserDto(user.Id, user.FirstName, user.Email, user.CreatedAt));
        });

        group.MapGet("/{id}/groups", async (int id, AppDbContext db) =>
        {
            var groups = await db.Memberships
                .Where(m => m.UserId == id)
                .Include(m => m.DevGroup)
                .Select(m => new DevGroupDto(m.DevGroup.Id, m.DevGroup.Name, m.DevGroup.Identifier, m.DevGroup.CreatedAt))
                .ToListAsync();

            return Results.Ok(groups);
        });
    }
}
