using System.Windows;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModels.Dialogs;
using HotelManagementSystem.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Services.Dialogs;

public sealed class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public BookingPreview? ShowReservationDialog()
    {
        var owner = Application.Current.MainWindow
            ?? throw new InvalidOperationException(
                "The application's main window has not been created.");

        var viewModel = _serviceProvider
            .GetRequiredService<ReservationEditorViewModel>();

        var dialog = new ReservationEditorDialog(viewModel)
        {
            Owner = owner
        };

        return dialog.ShowDialog() == true
            ? viewModel.CreatedBooking
            : null;
    }
}
