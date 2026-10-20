namespace HotelManagementSystem.Services.Reservations;

public sealed record CreateReservationRequest(
    string GuestName,
    DateTime CheckIn,
    DateTime CheckOut,
    int GuestCount,
    string RoomNumber);