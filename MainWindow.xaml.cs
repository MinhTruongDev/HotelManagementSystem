using HotelManagementSystem.Views;
using System.Windows;

namespace HotelManagementSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow(ShellView shellView)
        {
            InitializeComponent();
            Content = shellView;
        }
    }
}