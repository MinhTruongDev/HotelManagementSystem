
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Reservations;
using System.Collections.ObjectModel;

namespace HotelManagementSystem.ViewModels.Dialogs;

public partial class ReservationEditorViewModel : ViewModelBase
{
    private readonly IReservationService _reservationService;

    public ObservableCollection<RoomModel> AvailableRooms { get; } = [];

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

    public event Action<bool>? RequestClose;

    public ReservationEditorViewModel(IReservationService reservationService)
    {
        _reservationService = reservationService;
        _ = RefreshAvailableRoomsAsync();
    }

    partial void OnCheckInDateChanged(DateTime value)
    {
        _ = RefreshAvailableRoomsAsync();
    }

    partial void OnCheckOutDateChanged(DateTime value)
    {
        _ = RefreshAvailableRoomsAsync();
    }

    partial void OnGuestCountChanged(int value)
    {
        _ = RefreshAvailableRoomsAsync();
    }

    private async Task RefreshAvailableRoomsAsync()
    {
        var previouslySelectedRoom = SelectedRoom?.Number;

        try
        {
            if (CheckOutDate.Date <= CheckInDate.Date ||
                CheckInDate.Date < DateTime.Today ||
                GuestCount < 1)
            {
                AvailableRooms.Clear();
                SelectedRoom = null;
                return;
            }

            var rooms = await _reservationService.GetAvailableRoomsAsync(
                CheckInDate.Date,
                CheckOutDate.Date,
                GuestCount);

            AvailableRooms.Clear();

            foreach (var room in rooms)
                AvailableRooms.Add(room);

            SelectedRoom = AvailableRooms.FirstOrDefault(
                room => room.Number == previouslySelectedRoom)
                ?? AvailableRooms.FirstOrDefault();
        }
        catch (Exception ex)
        {
            AvailableRooms.Clear();
            SelectedRoom = null;
            ValidationMessage = $"Error fetching available rooms: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
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

        try
        {
            var request = new CreateReservationRequest(
                GuestName.Trim(),
                CheckInDate.Date,
                CheckOutDate.Date,
                GuestCount,
                SelectedRoom.Number);
            CreatedBooking = await _reservationService.CreateReservationAsync(request);

            RequestClose?.Invoke(true);
        }
        catch (Exception ex)
        {
            ValidationMessage = $"Error saving booking: {ex.Message}";
            await RefreshAvailableRoomsAsync();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CreatedBooking = null;
        RequestClose?.Invoke(false);
    }
}
