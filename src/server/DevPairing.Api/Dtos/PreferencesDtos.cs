namespace DevPairing.Api.Dtos;

public record UserPreferencesDto(
    bool NotifyOnSlotJoin,
    bool NotifyBeforeSessionStart
);

public record UpdateUserPreferencesDto(
    bool NotifyOnSlotJoin,
    bool NotifyBeforeSessionStart
);
