using System.Windows;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModels.Dialogs;
using HotelManagementSystem.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Services.Dialogs;

public sealed class DialogService : IDialogService
{
    private readonly IReservationEditorDialogFactory _dialogFactory;

    public DialogService(IReservationEditorDialogFactory dialogFactory)
    {
        _dialogFactory = dialogFactory;
    }

    public BookingPreview? ShowReservationDialog()
    {
        var dialog = _dialogFactory.Create();

        var owner = Application.Current.MainWindow;

        if (owner is not null)
        {
            dialog.Owner = owner;
        }

        return dialog.ShowDialog() == true
            ? ((ReservationEditorViewModel)dialog.DataContext)
                .CreatedBooking
            : null;
    }
}
