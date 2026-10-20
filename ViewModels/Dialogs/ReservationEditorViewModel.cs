
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.ViewModels.Dialogs;

public partial class ReservationEditorViewModel : ViewModelBase
{
    private readonly PrototypeData _prototypeData;

    public ObservableCollection<RoomModel> AvailableRooms
    { get; } = [];

    [ObservableProperty]
    private string guestName = string.Empty;

    [ObservableProperty]
    private DateTime checkInDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private DateTime checkOutDate = DateTime.Today.AddDays(2);

    [ObservableProperty]
    private int guestCount = 1;

    [ObservableProperty]
    private RoomModel? selectedRoom;

    [ObservableProperty]
    private string? validationMessage;

    public BookingPreview? CreatedBooking { get; private set; }

    // The Window listens to this event and closes itself.
    public event Action<bool>? RequestClose;

    public ReservationEditorViewModel(PrototypeData prototypeData)
    {
        _prototypeData = prototypeData;
        RefreshAvailableRooms();
    }

    partial void OnCheckInDateChanged(DateTime value)
    {
        RefreshAvailableRooms();
    }

    partial void OnCheckOutDateChanged(DateTime value)
    {
        RefreshAvailableRooms();
    }

    partial void OnGuestCountChanged(int value)
    {
        RefreshAvailableRooms();
    }

    private void RefreshAvailableRooms()
    {
        var previouslySelectedRoom = SelectedRoom?.Number;

        var rooms = _prototypeData.Rooms.Where(room => int.TryParse(room.Capacity, out var capacity) && capacity >= GuestCount)
            .Where(_ => CheckOutDate.Date > CheckInDate.Date)
            .Where(IsRoomAvailable)
            .ToList();

        AvailableRooms.Clear();

        foreach (var room in rooms)
            AvailableRooms.Add(room);

        SelectedRoom = AvailableRooms.FirstOrDefault(
            room => room.Number == previouslySelectedRoom)
            ?? AvailableRooms.FirstOrDefault();
    }

    private bool IsRoomAvailable(RoomModel room)
    {
        var maintenancePeriods = _prototypeData.MaintenancePeriods
            .Where(period => period.RoomNumber == room.Number)
            .ToList();

        if (room.Status == "Maintenance" && maintenancePeriods.Count == 0)
            return false;

        var overlapsMaintenance = maintenancePeriods.Any(period =>
            CheckInDate.Date < period.EndDate.Date &&
            CheckOutDate.Date > period.StartDate.Date);

        if (overlapsMaintenance)
            return false;

        if (room.Status == "Occupied")
        {
            if (room.OccupiedUntil is null)
                return false;

            if (CheckInDate.Date < room.OccupiedUntil.Value.Date)
                return false;
        }

        if (room.Status == "Needs Cleaning" &&
            CheckInDate.Date <= DateTime.Today)
        {
            return false;
        }

        var overlapsBooking = _prototypeData.AllBookings.Any(booking =>
            booking.Room == room.Number &&
            booking.Status != "Cancelled" &&
            CheckInDate.Date < booking.CheckOut.Date &&
            CheckOutDate.Date > booking.CheckIn.Date);

        return !overlapsBooking;
    }

    [RelayCommand]
    private void Save()
    {
        ValidationMessage = null;

        if (string.IsNullOrWhiteSpace(GuestName))
        {
            ValidationMessage = "Please enter the guest's name.";
            return;
        }

        if (CheckInDate.Date < DateTime.Today)
        {
            ValidationMessage = "Check-in cannot be in the past.";
            return;
        }

        if (CheckOutDate.Date <= CheckInDate.Date)
        {
            ValidationMessage = "Check-out must be after check-in.";
            return;
        }

        if (GuestCount < 1)
        {
            ValidationMessage = "Guest count must be at least one.";
            return;
        }

        if (SelectedRoom is null ||
            !AvailableRooms.Any(room =>
                room.Number == SelectedRoom.Number))
        {
            ValidationMessage = "Please select an available room.";
            return;
        }

        // Recheck availability immediately before creating the booking.
        if (!IsRoomAvailable(SelectedRoom))
        {
            ValidationMessage =
                "That room is no longer available for these dates.";
            RefreshAvailableRooms();
            return;
        }

        var reference = GenerateNextReference();

        CreatedBooking = new BookingPreview(
            reference,
            GuestName.Trim(),
            SelectedRoom.Number,
            CheckInDate.Date,
            CheckOutDate.Date,
            GuestCount,
            "Confirmed");

        RequestClose?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        CreatedBooking = null;
        RequestClose?.Invoke(false);
    }

    private string GenerateNextReference()
    {
        var nextNumber = _prototypeData.AllBookings
            .Select(booking =>
                booking.Reference.StartsWith(
                    "BK-", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(
                    booking.Reference.AsSpan(3),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var number)
                    ? number
                    : 0)
            .DefaultIfEmpty(2047)
            .Max() + 1;

        return $"BK-{nextNumber}";
    }
}
