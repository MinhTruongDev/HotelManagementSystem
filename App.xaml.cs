
using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Navigation;
using HotelManagementSystem.Services.Reservations;
using HotelManagementSystem.ViewModels;
using HotelManagementSystem.ViewModels.Pages;
using HotelManagementSystem.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HotelManagementSystem
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            var mainWindow = _serviceProvider
                .GetRequiredService<MainWindow>();

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Existing prototype data used during development.
            services.AddSingleton<PrototypeData>();

            // Navigation.
            services.AddSingleton<INavigationService, NavigationService>();

            // Shell.
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<ShellView>();
            services.AddSingleton<MainWindow>();

            // Page ViewModels.
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<BookingsViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<PrototypeData>();

            services.AddSingleton<IReservationService, PrototypeReservationService>();

            //services.AddSingleton<RoomsViewModel>();
            //services.AddSingleton<EmployeesViewModel>();
            //services.AddSingleton<AccountsViewModel>();
            //services.AddSingleton<SettingsViewModel>();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
