using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Reservations
{
    class PrototypeReservationService : IReservationService
    {
        public Task<PrototypeData.BookingPreview> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PrototypeData.RoomModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PrototypeData.BookingPreview>> GetBookingsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
