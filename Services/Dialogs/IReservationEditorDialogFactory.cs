using HotelManagementSystem.Views.Dialogs;

namespace HotelManagementSystem.Services.Dialogs;

public interface IReservationEditorDialogFactory
{
    ReservationEditorDialog Create();
}