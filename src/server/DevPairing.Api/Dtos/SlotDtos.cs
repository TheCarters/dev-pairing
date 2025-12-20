namespace DevPairing.Api.Dtos;

public record PairingSlotDto(
    int Id,
    int DevGroupId,
    UserDto Owner,
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string? Description,
    string? NtfyTopic,
    List<PairingSignupDto> Signups
);

public record CreatePairingSlotDto(
    int DevGroupId,
    int OwnerId,
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string? Description
);

public record UpdatePairingSlotDto(
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string? Description
);

public record PairingSlotSummaryDto(
    int Id,
    int DevGroupId,
    UserDto Owner,
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string? Description,
    string? NtfyTopic
);
