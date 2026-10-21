
using System;
using HotelManagementSystem.ViewModels.Dialogs;
using HotelManagementSystem.Views.Dialogs;

namespace HotelManagementSystem.Services.Dialogs;

public class ReservationEditorDialogFactory
    : IReservationEditorDialogFactory
{
    private readonly Func<ReservationEditorViewModel> _viewModelFactory;

    public ReservationEditorDialogFactory(
        Func<ReservationEditorViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public ReservationEditorDialog Create()
    {
        var viewModel = _viewModelFactory();
        return new ReservationEditorDialog(viewModel);
    }
}
