using DevPairing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPairing.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        if (!await db.DevGroups.AnyAsync())
        {
            db.DevGroups.Add(new DevGroup
            {
                Name = "General Engineering",
                Identifier = Guid.NewGuid(),
                IsActive = true
            });

            await db.SaveChangesAsync();
        }
    }
}
