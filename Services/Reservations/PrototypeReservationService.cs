using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Reservations;

public sealed class PrototypeReservationService : IReservationService
{
    private readonly PrototypeData _prototypeData;

    public PrototypeReservationService(PrototypeData prototypeData)
    {
        _prototypeData = prototypeData;
    }

    public Task<IReadOnlyList<BookingPreview>> GetBookingsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<BookingPreview> bookings =
            _prototypeData.AllBookings.ToList();

        return Task.FromResult(bookings);
    }

    public Task<IReadOnlyList<RoomModel>> GetAvailableRoomsAsync(
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (checkIn.Date < DateTime.Today ||
            checkOut.Date <= checkIn.Date ||
            guestCount < 1)
        {
            IReadOnlyList<RoomModel> noRooms = Array.Empty<RoomModel>();
            return Task.FromResult(noRooms);
        }

        IReadOnlyList<RoomModel> rooms = _prototypeData.Rooms
            .Where(room =>
                int.TryParse(room.Capacity, out var capacity) &&
                capacity >= guestCount)
            .Where(room => IsRoomAvailable(room, checkIn, checkOut))
            .ToList();

        return Task.FromResult(rooms);
    }

    public Task<BookingPreview> CreateReservationAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.GuestName))
        {
            throw new ArgumentException(
                "Guest name is required.",
                nameof(request));
        }

        if (request.CheckIn.Date < DateTime.Today)
        {
            throw new ArgumentException(
                "Check-in cannot be in the past.",
                nameof(request));
        }

        if (request.CheckOut.Date <= request.CheckIn.Date)
        {
            throw new ArgumentException(
                "Check-out must be after check-in.",
                nameof(request));
        }

        if (request.GuestCount < 1)
        {
            throw new ArgumentException(
                "Guest count must be at least one.",
                nameof(request));
        }

        var room = _prototypeData.Rooms.FirstOrDefault(
            room => room.Number == request.RoomNumber);

        if (room is null)
        {
            throw new InvalidOperationException(
                "The selected room does not exist.");
        }

        if (!int.TryParse(room.Capacity, out var capacity) ||
            capacity < request.GuestCount)
        {
            throw new InvalidOperationException(
                "The selected room cannot accommodate this many guests.");
        }

        if (!IsRoomAvailable(room, request.CheckIn, request.CheckOut))
        {
            throw new InvalidOperationException(
                "The selected room is not available for these dates.");
        }

        var nextNumber = _prototypeData.AllBookings
            .Select(booking =>
                booking.Reference.StartsWith(
                    "BK-", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(booking.Reference.AsSpan(3), out var number)
                    ? number
                    : 0)
            .DefaultIfEmpty(2047)
            .Max() + 1;

        var booking = new BookingPreview(
            $"BK-{nextNumber}",
            request.GuestName.Trim(),
            room.Number,
            request.CheckIn.Date,
            request.CheckOut.Date,
            request.GuestCount,
            "Confirmed");

        _prototypeData.AllBookings.Add(booking);

        return Task.FromResult(booking);
    }

    private bool IsRoomAvailable(
        RoomModel room,
        DateTime checkIn,
        DateTime checkOut)
    {
        var maintenancePeriods = _prototypeData.MaintenancePeriods
            .Where(period => period.RoomNumber == room.Number)
            .ToList();

        if (room.Status == "Maintenance" &&
            maintenancePeriods.Count == 0)
        {
            return false;
        }

        var overlapsMaintenance = maintenancePeriods.Any(period =>
            checkIn.Date < period.EndDate.Date &&
            checkOut.Date > period.StartDate.Date);

        if (overlapsMaintenance)
        {
            return false;
        }

        if (room.Status == "Occupied")
        {
            if (room.OccupiedUntil is null ||
                checkIn.Date < room.OccupiedUntil.Value.Date)
            {
                return false;
            }
        }

        if (room.Status == "Needs Cleaning" &&
            checkIn.Date <= DateTime.Today)
        {
            return false;
        }

        var overlapsBooking = _prototypeData.AllBookings.Any(booking =>
            booking.Room == room.Number &&
            !string.Equals(
                booking.Status,
                "Cancelled",
                StringComparison.OrdinalIgnoreCase) &&
            checkIn.Date < booking.CheckOut.Date &&
            checkOut.Date > booking.CheckIn.Date);

        return !overlapsBooking;
    }
}
