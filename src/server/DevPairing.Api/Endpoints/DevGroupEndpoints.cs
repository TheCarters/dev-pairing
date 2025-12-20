using DevPairing.Api.Data;
using DevPairing.Api.Dtos;
using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Endpoints;

public static class DevGroupEndpoints
{
    public static void MapDevGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/groups").WithTags("DevGroups");

        group.MapPost("/", async (CreateDevGroupDto input, AppDbContext db) =>
        {
            var devGroup = new DevGroup
            {
                Name = input.Name,
                Identifier = Guid.NewGuid(),
                IsActive = true
            };

            db.DevGroups.Add(devGroup);
            await db.SaveChangesAsync();

            return Results.Created($"/api/groups/{devGroup.Identifier}", 
                new DevGroupDto(devGroup.Id, devGroup.Name, devGroup.Identifier, devGroup.CreatedAt));
        });

        group.MapGet("/{identifier}", async (Guid identifier, AppDbContext db) =>
        {
            var devGroup = await db.DevGroups
                .FirstOrDefaultAsync(g => g.Identifier == identifier);

            if (devGroup == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new DevGroupDto(devGroup.Id, devGroup.Name, devGroup.Identifier, devGroup.CreatedAt));
        });

        group.MapGet("/{id}/members", async (int id, AppDbContext db) =>
        {
            var members = await db.Memberships
                .Where(m => m.DevGroupId == id)
                .Include(m => m.User)
                .Select(m => new UserDto(m.User.Id, m.User.FirstName, m.User.Email, m.User.CreatedAt))
                .ToListAsync();

            return Results.Ok(members);
        });
    }
}
