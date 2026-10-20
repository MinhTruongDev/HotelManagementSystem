using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagementSystem.ViewModels;

public partial class LoginViewModel: ViewModelBase
{
    [ObservableProperty]
    private string userName = "reception@staywise.com";
    [ObservableProperty]
    private string password = string.Empty;
    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    private void SignIn(){
        if (string.IsNullOrWhiteSpace(userName)
            || string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Enter username or password to enter.";
        }
    }



}