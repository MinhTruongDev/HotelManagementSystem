using CommunityToolkit.Mvvm.Input;
using HotelManagementSystem.Services.Navigation;

namespace HotelManagementSystem.ViewModels.Pages;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public DashboardViewModel(
        INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void NewBooking()
    {
        //_navigationService.Navigate(PageKey.Rooms);
    }

    [RelayCommand]
    private void ManageRoom()
    {
       // _navigationService.Navigate(PageKey.Rooms);
    }
}