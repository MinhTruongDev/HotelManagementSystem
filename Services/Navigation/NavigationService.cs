using Microsoft.Extensions.DependencyInjection;
using HotelManagementSystem.ViewModels;
using HotelManagementSystem.ViewModels.Pages;

namespace HotelManagementSystem.Services.Navigation;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public object? CurrentPage { get; private set; }

    public PageKey CurrentPageKey { get; private set; } = PageKey.Dashboard;

    public event EventHandler? CurrentPageChanged;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Navigate(PageKey page)
    {
        object nextPage = page switch
        {
            PageKey.Dashboard => _serviceProvider.GetRequiredService<DashboardViewModel>(),
            PageKey.Bookings => _serviceProvider.GetRequiredService<BookingsViewModel>(),
            //PageKey.Rooms => _serviceProvider.GetRequiredService<RoomsViewModel>(),
            //PageKey.Employees => _serviceProvider.GetRequiredService<EmployeesViewModel>(),
            //PageKey.Accounts => _serviceProvider.GetRequiredService<AccountsViewModel>(),
            PageKey.Settings => _serviceProvider.GetRequiredService<SettingsViewModel>(),
            _ => throw new ArgumentOutOfRangeException(nameof(page), page, "Unknown navigation target.")
        };

        CurrentPage = nextPage;
        CurrentPageKey = page;
        CurrentPageChanged?.Invoke(this, EventArgs.Empty);
    }
}
