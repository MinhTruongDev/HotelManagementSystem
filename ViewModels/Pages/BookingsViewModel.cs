using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagementSystem.ViewModels.Pages;

public partial class BookingsViewModel:ViewModelBase
{
    public BookingsViewModel()
    {
    }
   

    [ObservableProperty]
    private object? selectedBooking;
    [ObservableProperty]
    private string searchText = string.Empty;
    [ObservableProperty]
    private bool isLoading;
    [ObservableProperty]
    private string? errorMessage;


    //[RelayCommand]
    //private void ReservationStatus()
    //{
    //    if (_prototypeData.AllBookings.Count == 0) return;
    //    var current = _prototypeData.AllBookings[0];
    //    var next = current.Status switch
    //    {
    //        "Reserved" => "Checked In", "Checked In" => "Checked Out", "Checked Out" => "Cancelled", _ => "Reserved"
    //    };
    //    _prototypeData.AllBookings[0] = current with { Status = next };
    //    MessageBox.Show($"{current.Guest} is now marked as {next} in the prototype.", "Reservation status", MessageBoxButton.OK, MessageBoxImage.Information);
    //}

    //[RelayCommand]
    //private void NewBooking() => OpenReservationDialog(null);   

    //private void OpenReservationDialog(RoomModel? preselectedRoom)
    //{
    //}

    //partial void OnReservationSearchChanged(string value)
    //{
    //    FilterReservations();
    //}

    //partial void OnBookingsCollectionChanged(
    //    object? sender,
    //    NotifyCollectionChangedEventArgs e)
    //{
    //    RefreshBookings();
    //}

    //private void RefreshBookings()
    //{
    //    var text = ReservationSearch.Trim();
    //    var results = _prototypeData.AllBookings.Where(booking => 
    //        string.IsNullOrWhiteSpace(text)
    //        || booking.Reference.Contains(text, StringComparison.OrdinalIgnoreCase)
    //        || booking.Guest.Contains(text, StringComparison.OrdinalIgnoreCase));
    //    FilteredBooking.Clear();
    //    foreach(var booking in results)
    //    {
    //        FilteredBooking.Add(booking);
    //    }
        
    //}
}