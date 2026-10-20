using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagementSystem.Services.Navigation;

namespace HotelManagementSystem.ViewModels;

public enum PageKey
{
    Dashboard,
    Bookings,
    //Rooms,
    //Employees,
    //Accounts,
    //Settings
}

public partial class ShellViewModel:ViewModelBase
{
    private readonly INavigationService _navigationService;

    public object? CurrentPage => _navigationService.CurrentPage;

    public PageKey CurrentPageKey => _navigationService.CurrentPageKey;

    public string CurrentBreadcrumb => CurrentPageKey switch
    {
        PageKey.Bookings => "Operations / Bookings & Reservations",
        //PageKey.Rooms => "Operations / Room Management",
        //PageKey.Employees => "Administration / Employee Management",
        //PageKey.Accounts => "Administration / Account Management",
        //PageKey.Settings => "Workspace / Settings",
        _ => "Overview / Dashboard"
    };   


    public ShellViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.CurrentPageChanged += OnCurrentPageChanged;
        _navigationService.Navigate(PageKey.Dashboard);
    }
    
    [RelayCommand]
    private void Navigate(PageKey page)
    {
        _navigationService.Navigate(page);
    }

    private void OnCurrentPageChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(CurrentPageKey));
        OnPropertyChanged(nameof(CurrentBreadcrumb));
    }
}