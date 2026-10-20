using System.Windows.Controls;
using HotelManagementSystem.ViewModels;

namespace HotelManagementSystem.Views;

public partial class ShellView : UserControl
{
    public ShellView(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}