using DevPairing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPairing.Api.Endpoints;

public record AdminNotifyDto(string Topic, string Message);

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin");

        group.MapPost("/notify", async ([FromBody] AdminNotifyDto input, INtfyService ntfyService) =>
        {
            await ntfyService.SendNotificationAsync(input.Topic, "Admin Test", input.Message);
            return Results.Ok();
        });
    }
}
