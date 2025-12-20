namespace DevPairing.Api.Dtos;

public record DevGroupDto(int Id, string Name, Guid Identifier, DateTime CreatedAt);

public record CreateDevGroupDto(string Name);
