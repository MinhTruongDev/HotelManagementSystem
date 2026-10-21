
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HotelManagementSystem.Services.Dialogs;

namespace HotelManagementSystem.ViewModels.Pages;

public partial class BookingsViewModel : ViewModelBase
{
    private readonly PrototypeData _prototypeData;
    private readonly IDialogService _dialogService;

    public ObservableCollection<string> StatusOptions { get; } =
    [
        "All statuses",
        "Confirmed",
        "Pending",
        "Cancelled"
    ];

    public ObservableCollection<BookingPreview> FilteredBookings { get; } = [];

    [ObservableProperty]
    private BookingPreview? selectedBooking;

    [ObservableProperty]
    private string reservationSearch = string.Empty;

    [ObservableProperty]
    private string selectedStatus = "All statuses";

    [ObservableProperty]
    private DateTime? selectedCheckInDate;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public BookingsViewModel(
        PrototypeData prototypeData,
        IDialogService dialogService)
    {
        _prototypeData = prototypeData;
        _dialogService = dialogService;

        _prototypeData.AllBookings.CollectionChanged +=
            OnBookingsCollectionChanged;

        RefreshBookings();
    }

    partial void OnReservationSearchChanged(string value)
        => RefreshBookings();

    partial void OnSelectedStatusChanged(string value)
        => RefreshBookings();

    partial void OnSelectedCheckInDateChanged(DateTime? value)
        => RefreshBookings();

    private void OnBookingsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        RefreshBookings();
    }

    private void RefreshBookings()
    {
        var search = ReservationSearch.Trim();

        var results = _prototypeData.AllBookings.Where(booking =>
            (string.IsNullOrWhiteSpace(search)
             || booking.Reference.Contains(
                 search, StringComparison.OrdinalIgnoreCase)
             || booking.Guest.Contains(
                 search, StringComparison.OrdinalIgnoreCase)
             || booking.Room.Contains(
                 search, StringComparison.OrdinalIgnoreCase))
            &&
            (SelectedStatus == "All statuses"
             || booking.Status == SelectedStatus)
            &&
            (!SelectedCheckInDate.HasValue
             || booking.CheckIn.Date == SelectedCheckInDate.Value.Date))
            .ToList();

        FilteredBookings.Clear();

        foreach (var booking in results)
        {
            FilteredBookings.Add(booking);
        }
    }

    [RelayCommand]
    private void NewBooking()
    {
        ErrorMessage = null;
        _dialogService.ShowReservationDialog();
    }
}
