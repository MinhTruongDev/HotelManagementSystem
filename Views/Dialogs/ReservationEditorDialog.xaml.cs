
using System.Windows;
using HotelManagementSystem.ViewModels.Dialogs;

namespace HotelManagementSystem.Views.Dialogs;

public partial class ReservationEditorDialog : Window
{
    public ReservationEditorDialog(ReservationEditorViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
        viewModel.RequestClose += OnRequestClose;
        Closed += (_, _) => viewModel.RequestClose -= OnRequestClose;
    }

    private void OnRequestClose(bool result)
    {
        DialogResult = result;
    }
}
