using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Endpoints;

public static class PreferencesEndpoints
{
    public static void MapPreferencesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/{userId}/preferences").WithTags("Preferences");

        group.MapGet("/", async (int userId, AppDbContext db) =>
        {
            var prefs = await db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            
            if (prefs == null)
            {
                // Create default if missing (should exist from User creation, but for safety)
                var user = await db.Users.FindAsync(userId);
                if (user == null) return Results.NotFound("User not found");

                prefs = new UserPreferences { UserId = userId };
                db.UserPreferences.Add(prefs);
                await db.SaveChangesAsync();
            }

            return Results.Ok(new UserPreferencesDto(prefs.NotifyOnSlotJoin, prefs.NotifyBeforeSessionStart));
        });

        group.MapPut("/", async (int userId, UpdateUserPreferencesDto input, AppDbContext db) =>
        {
            var prefs = await db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            
            if (prefs == null)
            {
                 var user = await db.Users.FindAsync(userId);
                 if (user == null) return Results.NotFound("User not found");
                 
                 prefs = new UserPreferences { UserId = userId };
                 db.UserPreferences.Add(prefs);
            }

            prefs.NotifyOnSlotJoin = input.NotifyOnSlotJoin;
            prefs.NotifyBeforeSessionStart = input.NotifyBeforeSessionStart;

            await db.SaveChangesAsync();

            return Results.Ok(new UserPreferencesDto(prefs.NotifyOnSlotJoin, prefs.NotifyBeforeSessionStart));
        });
    }
}
