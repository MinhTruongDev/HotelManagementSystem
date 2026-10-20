using HotelManagementSystem.ViewModels;

namespace HotelManagementSystem.Services.Navigation;

public interface INavigationService
{
    object? CurrentPage { get; }
    PageKey CurrentPageKey { get; }
    event EventHandler? CurrentPageChanged;
    void Navigate(PageKey key);
}   