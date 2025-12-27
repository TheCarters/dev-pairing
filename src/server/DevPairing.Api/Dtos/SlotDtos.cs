namespace DevPairing.Api.Dtos;

public record PairingSlotDto(
    int Id,
    int DevGroupId,
    UserDto Owner,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Title,
    string? Description,
    string? NtfyTopic,
    List<PairingSignupDto> Signups
);

public record CreatePairingSlotDto(
    int DevGroupId,
    int OwnerId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Title,
    string? Description
);

public record UpdatePairingSlotDto(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Title,
    string? Description
);

public record PairingSlotSummaryDto(
    int Id,
    int DevGroupId,
    UserDto Owner,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Title,
    string? Description,
    string? NtfyTopic
);
