
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Reservations;

public interface IReservationService
{
    Task<IReadOnlyList<BookingPreview>> GetBookingsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomModel>> GetAvailableRoomsAsync(
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        CancellationToken cancellationToken = default);

    Task<BookingPreview> CreateReservationAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default);
}
