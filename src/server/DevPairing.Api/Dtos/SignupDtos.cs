namespace DevPairing.Api.Dtos;

public record PairingSignupDto(
    int Id,
    int SlotId,
    UserDto User,
    DateTime SignedUpAt,
    string Status
);

public record CreatePairingSignupDto(
    int SlotId,
    int UserId
);

public record PairingSignupWithSlotDto(
    int Id,
    PairingSlotSummaryDto Slot,
    UserDto User,
    DateTimeOffset SignedUpAt,
    string Status
);
