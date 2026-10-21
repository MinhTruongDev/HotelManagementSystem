
using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Dialogs;
using HotelManagementSystem.Services.Navigation;
using HotelManagementSystem.Services.Reservations;
using HotelManagementSystem.ViewModels;
using HotelManagementSystem.ViewModels.Dialogs;
using HotelManagementSystem.ViewModels.Pages;
using HotelManagementSystem.Views;
using HotelManagementSystem.Views.Dialogs;
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

            // Reservation services.
            services.AddSingleton<IReservationService, PrototypeReservationService>();
            services.AddTransient<ReservationEditorViewModel>();

            // Dialog services and factory.
            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<Func<ReservationEditorViewModel>>(provider =>
                () => provider.GetRequiredService<ReservationEditorViewModel>());

            services.AddTransient<IReservationEditorDialogFactory,
                ReservationEditorDialogFactory>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
