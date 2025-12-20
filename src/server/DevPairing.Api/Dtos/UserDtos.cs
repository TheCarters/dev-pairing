namespace DevPairing.Api.Dtos;

public record UserDto(int Id, string FirstName, string Email, DateTime CreatedAt);

public record CreateUserDto(string FirstName, string Email, string? JoinGroupId = null);

public record JoinGroupDto(int UserId, Guid GroupIdentifier);
