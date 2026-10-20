using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Reservations
{
    class PrototypeReservationService : IReservationService
    {
        public Task<BookingPreview> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RoomModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BookingPreview>> GetBookingsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
