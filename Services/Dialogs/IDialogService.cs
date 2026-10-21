using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Dialogs;

public interface IDialogService
{
    BookingPreview? ShowReservationDialog();
}
