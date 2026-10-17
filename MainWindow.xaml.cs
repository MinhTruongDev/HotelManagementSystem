using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HotelManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _sidebarOpen;
        private Button? _selectedNavButton;

        private PrototypeData Prototype => (PrototypeData)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new PrototypeData();
        }

        private void ReservationStatus_Click(object sender, RoutedEventArgs e)
        {
            if (Prototype.AllBookings.Count == 0) return;
            var current = Prototype.AllBookings[0];
            var next = current.Status switch { "Reserved" => "Checked In", "Checked In" => "Checked Out", "Checked Out" => "Cancelled", _ => "Reserved" };
            Prototype.AllBookings[0] = current with { Status = next };
            MessageBox.Show($"{current.Guest} is now marked as {next} in the prototype.", "Reservation status", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not EmployeeModel employee) { MessageBox.Show("Select an employee first.", "Employee management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            var name = Field("Full name", employee.Name);
            ShowEditor("Edit employee", new (string, Control)[] { ("Full name", name) }, () =>
            {
                if (string.IsNullOrWhiteSpace(name.Text)) return "A name is required.";
                Prototype.Employees[Prototype.Employees.IndexOf(employee)] = employee with { Name = name.Text };
                return null;
            });
        }

        private void DeactivateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not EmployeeModel employee) { MessageBox.Show("Select an employee first.", "Employee management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (MessageBox.Show($"Deactivate {employee.Name}?", "Confirm employee change", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var index = Prototype.Employees.IndexOf(employee);
                Prototype.Employees[index] = employee with { Status = "Inactive" };
            }
        }

        private void EditFloor_Click(object sender, RoutedEventArgs e)
        {
            if (Prototype.Floors.Count == 0) return;
            var original = Prototype.Floors[0];
            var floor = Field("Floor number", original);
            ShowEditor("Edit floor", new (string, Control)[] { ("Floor number", floor) }, () =>
            {
                if (string.IsNullOrWhiteSpace(floor.Text)) return "Enter a floor number.";
                Prototype.Floors[0] = floor.Text;
                return null;
            });
        }

        private void DeactivateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountGrid.SelectedItem is not AccountModel account) { MessageBox.Show("Select an account first.", "Account management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (MessageBox.Show($"Deactivate {account.Username}?", "Confirm account change", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var index = Prototype.Accounts.IndexOf(account);
                Prototype.Accounts[index] = account with { Status = "Inactive" };
            }
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountGrid.SelectedItem is not AccountModel account) { MessageBox.Show("Select an account first.", "Account management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            var username = Field("Username", account.Username);
            var role = new ComboBox { ItemsSource = new[] { "Admin", "Manager", "Receptionist" }, SelectedItem = account.Role, Padding = new Thickness(8) };
            ShowEditor("Edit account", new (string, Control)[] { ("Username", username), ("Role", role) }, () =>
            {
                if (string.IsNullOrWhiteSpace(username.Text)) return "Username is required.";
                Prototype.Accounts[Prototype.Accounts.IndexOf(account)] = account with { Username = username.Text, Role = role.Text };
                return null;
            });
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                LoginMessage.Text = "Enter your username and password to continue.";
                return;
            }

            LoginPanel.Visibility = Visibility.Collapsed;
            ShellPanel.Visibility = Visibility.Visible;
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            _sidebarOpen = !_sidebarOpen;
            if (SidebarBorder.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform(-230, 0);
                SidebarBorder.RenderTransform = transform;
            }

            var animation = new DoubleAnimation
            {
                From = transform.X,
                To = _sidebarOpen ? 0 : -230,
                Duration = TimeSpan.FromMilliseconds(450),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            if (button.Tag?.ToString() == "Logout")
            {
                ShellPanel.Visibility = Visibility.Collapsed;
                LoginPanel.Visibility = Visibility.Visible;
                return;
            }

            if (_selectedNavButton is not null)
                _selectedNavButton.ClearValue(Control.BackgroundProperty);
            button.Background = new SolidColorBrush(Color.FromRgb(42, 60, 96));
            _selectedNavButton = button;
            ShowPage(button.Tag?.ToString() ?? "Dashboard");
        }

        private void ShowPage(string page)
        {
            ContentPages.Visibility = Visibility.Visible;
            DashboardPage.Visibility = page == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
            BookingsPage.Visibility = page == "Bookings" ? Visibility.Visible : Visibility.Collapsed;
            RoomsPage.Visibility = page == "Rooms" ? Visibility.Visible : Visibility.Collapsed;
            EmployeesPage.Visibility = page == "Employees" ? Visibility.Visible : Visibility.Collapsed;
            AccountsPage.Visibility = page == "Accounts" ? Visibility.Visible : Visibility.Collapsed;
            SettingsPage.Visibility = page == "Settings" ? Visibility.Visible : Visibility.Collapsed;
            Breadcrumb.Text = page switch
            {
                "Bookings" => "Operations  /  Bookings & Reservations",
                "Rooms" => "Operations  /  Room Management",
                "Employees" => "Administration  /  Employee Management",
                "Accounts" => "Administration  /  Account Management",
                "Settings" => "Workspace  /  Settings",
                _ => "Overview  /  Dashboard"
            };
        }

        private void BookingsShortcut_Click(object sender, RoutedEventArgs e) => ShowPage("Bookings");

        private void RoomsShortcut_Click(object sender, RoutedEventArgs e) => ShowPage("Rooms");

        private void ReservationDetails_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Reservation details are available in this mock workflow. Check-in, check-out, cancellation, and no-show states are demonstrated by the status control.", "Reservation details", MessageBoxButton.OK, MessageBoxImage.Information);

        private void ReservationSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ReservationSearchBox.Text.Trim();
            ReservationGrid.ItemsSource = Prototype.AllBookings.Where(booking => string.IsNullOrWhiteSpace(text) || booking.Reference.Contains(text, StringComparison.OrdinalIgnoreCase) || booking.Guest.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        private void RoomSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = RoomSearchBox.Text.Trim();
            RoomGrid.ItemsSource = Prototype.Rooms.Where(room => string.IsNullOrWhiteSpace(text) || room.Number.Contains(text, StringComparison.OrdinalIgnoreCase) || room.Type.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        private void EmployeeSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = EmployeeSearchBox.Text.Trim();
            EmployeeGrid.ItemsSource = Prototype.Employees.Where(employee => string.IsNullOrWhiteSpace(text) || employee.Name.Contains(text, StringComparison.OrdinalIgnoreCase) || employee.Email.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        private void NewBooking_Click(object sender, RoutedEventArgs e) => OpenReservationDialog(null);

        private void RoomSelection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: RoomModel room })
                OpenReservationDialog(room);
        }

        private void OpenReservationDialog(RoomModel? preselectedRoom)
        {
            var guest = Field("Customer full name");
            var phone = Field("Phone number");
            var email = Field("Email (optional)");
            var checkIn = new DatePicker { SelectedDate = DateTime.Today, Margin = new Thickness(0, 4, 0, 8) };
            var checkOut = new DatePicker { SelectedDate = DateTime.Today.AddDays(2), Margin = new Thickness(0, 4, 0, 8) };
            var guests = Field("Guest count", "2");
            var room = new ComboBox { ItemsSource = Prototype.Rooms, DisplayMemberPath = "Number", SelectedItem = preselectedRoom ?? Prototype.Rooms.FirstOrDefault(), Margin = new Thickness(0, 4, 0, 8), Padding = new Thickness(8) };
            var notes = Field("Notes or special requests");
            var message = new TextBlock { Foreground = Brushes.Firebrick, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 8) };
            var save = new Button { Content = "Save reservation", Style = (Style)FindResource("PrimaryButton"), HorizontalAlignment = HorizontalAlignment.Right };
            var cancel = new Button { Content = "Cancel", Style = (Style)FindResource("QuietButton"), Margin = new Thickness(0, 0, 8, 0) };
            var dialog = Dialog("New reservation", 470);
            save.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(guest.Text) || string.IsNullOrWhiteSpace(phone.Text) || room.SelectedItem is not RoomModel selectedRoom || checkIn.SelectedDate is null || checkOut.SelectedDate is null || checkOut.SelectedDate <= checkIn.SelectedDate)
                {
                    message.Text = "Enter the required customer and room details. Check-out must be after check-in.";
                    return;
                }

                Prototype.AllBookings.Add(new BookingPreview($"RS-{2050 + Prototype.AllBookings.Count}", guest.Text, selectedRoom.Number, checkIn.SelectedDate.Value.ToString("dd MMM"), checkOut.SelectedDate.Value.ToString("dd MMM"), guests.Text, "Reserved"));
                RefreshReservationList();
                dialog.DialogResult = true;
            };
            cancel.Click += (_, _) => dialog.Close();
            dialog.Content = FormContent(dialog, new (string, Control)[] { ("Customer full name", guest), ("Phone number", phone), ("Email", email), ("Check-in", checkIn), ("Check-out", checkOut), ("Guest count", guests), ("Selected room", room), ("Notes", notes) }, message, save, cancel);
            dialog.ShowDialog();
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            var number = Field("Room number");
            var floor = Field("Floor", "1");
            var type = new ComboBox { ItemsSource = new[] { "Standard", "Deluxe", "Suite" }, SelectedIndex = 0, Padding = new Thickness(8) };
            var capacity = Field("Capacity", "2");
            var price = Field("Price per night", "145");
            var status = new ComboBox { ItemsSource = new[] { "Ready", "Occupied", "Needs Cleaning", "Maintenance", "Out of Service" }, SelectedIndex = 0, Padding = new Thickness(8) };
            ShowEditor("Add room", new (string, Control)[] { ("Room number", number), ("Floor", floor), ("Room type", type), ("Capacity", capacity), ("Price per night", price), ("Operational status", status) }, () =>
            {
                if (string.IsNullOrWhiteSpace(number.Text) || !decimal.TryParse(price.Text, out var roomPrice)) return "Room number and a valid price are required.";
                Prototype.Rooms.Add(new RoomModel(number.Text, floor.Text, type.Text, capacity.Text, roomPrice, status.Text));
                Prototype.RebuildRoomGroups();
                return null;
            });
        }

        private void RefreshReservationList() => ReservationGrid.ItemsSource = Prototype.AllBookings.ToList();

        private void EditSelectedRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomGrid.SelectedItem is not RoomModel room) { MessageBox.Show("Select a room first.", "Room management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            var price = Field("Price per night", room.Price.ToString("0"));
            var status = new ComboBox { ItemsSource = new[] { "Ready", "Occupied", "Needs Cleaning", "Maintenance", "Out of Service" }, SelectedItem = room.Status, Padding = new Thickness(8) };
            ShowEditor($"Edit room {room.Number}", new (string, Control)[] { ("Price per night", price), ("Operational status", status) }, () =>
            {
                if (!decimal.TryParse(price.Text, out var value)) return "Enter a valid price.";
                var index = Prototype.Rooms.IndexOf(room);
                Prototype.Rooms[index] = room with { Price = value, Status = status.Text };
                Prototype.RebuildRoomGroups();
                RoomGrid.ItemsSource = Prototype.Rooms.ToList();
                return null;
            });
        }

        private void DeactivateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomGrid.SelectedItem is not RoomModel room) { MessageBox.Show("Select a room first.", "Room management", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (MessageBox.Show($"Mark room {room.Number} as out of service?", "Confirm room change", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var index = Prototype.Rooms.IndexOf(room);
                Prototype.Rooms[index] = room with { Status = "Out of Service" };
                Prototype.RebuildRoomGroups();
                RoomGrid.ItemsSource = Prototype.Rooms.ToList();
            }
        }

        private void AddFloor_Click(object sender, RoutedEventArgs e)
        {
            var floor = Field("Floor number");
            ShowEditor("Add floor", new (string, Control)[] { ("Floor number", floor) }, () =>
            {
                if (string.IsNullOrWhiteSpace(floor.Text) || Prototype.Floors.Contains(floor.Text)) return "Enter a unique floor number.";
                Prototype.Floors.Add(floor.Text);
                return null;
            });
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var id = Field("Employee ID", $"EMP-{1040 + Prototype.Employees.Count}");
            var name = Field("Full name");
            var phone = Field("Phone number");
            var email = Field("Email");
            var position = new ComboBox { ItemsSource = new[] { "Receptionist", "Housekeeping", "Manager", "Operations" }, SelectedIndex = 0, Padding = new Thickness(8) };
            ShowEditor("Add employee", new (string, Control)[] { ("Employee ID", id), ("Full name", name), ("Phone number", phone), ("Email", email), ("Position", position) }, () =>
            {
                if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(email.Text)) return "Full name and email are required.";
                Prototype.Employees.Add(new EmployeeModel(id.Text, name.Text, phone.Text, email.Text, position.Text, "Active"));
                return null;
            });
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var username = Field("Username");
            var employee = new ComboBox { ItemsSource = Prototype.Employees, DisplayMemberPath = "Name", SelectedIndex = 0, Padding = new Thickness(8) };
            var role = new ComboBox { ItemsSource = new[] { "Admin", "Manager", "Receptionist" }, SelectedIndex = 2, Padding = new Thickness(8) };
            ShowEditor("Create account", new (string, Control)[] { ("Username", username), ("Employee", employee), ("Role", role) }, () =>
            {
                if (string.IsNullOrWhiteSpace(username.Text) || employee.SelectedItem is not EmployeeModel selected) return "Username and employee are required.";
                Prototype.Accounts.Add(new AccountModel($"ACC-{Prototype.Accounts.Count + 1:000}", username.Text, selected.Name, role.Text, "Active"));
                return null;
            });
        }

        private void RoomDetails_Click(object sender, RoutedEventArgs e)
        {
            if (Prototype.Rooms.Count == 0) return;
            var original = Prototype.Rooms[0];
            var price = Field("Price per night", original.Price.ToString("0"));
            var status = new ComboBox { ItemsSource = new[] { "Ready", "Occupied", "Needs Cleaning", "Maintenance", "Out of Service" }, SelectedItem = original.Status, Padding = new Thickness(8) };
            ShowEditor($"Edit room {original.Number}", new (string, Control)[] { ("Price per night", price), ("Operational status", status) }, () =>
            {
                if (!decimal.TryParse(price.Text, out var value)) return "Enter a valid price.";
                Prototype.Rooms[0] = original with { Price = value, Status = status.Text };
                return null;
            });
        }

        private void ThemeLight_Click(object sender, RoutedEventArgs e) => SetTheme(false);

        private void ThemeDark_Click(object sender, RoutedEventArgs e) => SetTheme(true);

        private void SetTheme(bool dark)
        {
            SetBrush("PageBrush", dark ? "#101827" : "#F4F6FA");
            SetBrush("TextBrush", dark ? "#F3F6FC" : "#172033");
            SetBrush("MutedBrush", dark ? "#AAB6C9" : "#738096");
            SetBrush("LineBrush", dark ? "#34435C" : "#E5E9F0");
            SetBrush("SidebarBrush", dark ? "#0B1425" : "#1C2B49");
            SetBrush("CardBrush", dark ? "#1B2638" : "#FFFFFF");
            SetBrush("ControlBrush", dark ? "#243149" : "#FFFFFF");
            MessageBox.Show(dark ? "Dark mode enabled." : "Light mode enabled.", "Appearance", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetBrush(string key, string color)
        {
            Application.Current.Resources[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        private static TextBox Field(string label, string text = "") => new() { Text = text, Tag = label, Margin = new Thickness(0, 4, 0, 8), Padding = new Thickness(9), MinWidth = 300 };

        private Window Dialog(string title, double width) => new() { Title = title, Width = width, SizeToContent = SizeToContent.Height, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, Background = (Brush)FindResource("PageBrush"), ResizeMode = ResizeMode.NoResize };

        private FrameworkElement FormContent(Window dialog, (string Label, Control Control)[] fields, TextBlock message, Button save, Button cancel)
        {
            var panel = new StackPanel { Margin = new Thickness(24) };
            foreach (var field in fields)
            {
                panel.Children.Add(new TextBlock { Text = field.Label, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 5, 0, 0) });
                panel.Children.Add(field.Control);
            }
            panel.Children.Add(message);
            var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            actions.Children.Add(cancel);
            actions.Children.Add(save);
            panel.Children.Add(actions);
            return panel;
        }

        private void ShowEditor(string title, (string Label, Control Control)[] fields, Func<string?> saveAction)
        {
            var message = new TextBlock { Foreground = Brushes.Firebrick, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 8) };
            var save = new Button { Content = "Save changes", Style = (Style)FindResource("PrimaryButton") };
            var cancel = new Button { Content = "Cancel", Style = (Style)FindResource("QuietButton"), Margin = new Thickness(0, 0, 8, 0) };
            var dialog = Dialog(title, 440);
            save.Click += (_, _) =>
            {
                var error = saveAction();
                if (error is not null) { message.Text = error; return; }
                dialog.DialogResult = true;
            };

            cancel.Click += (_, _) => dialog.Close();
            dialog.Content = FormContent(dialog, fields, message, save, cancel);
            dialog.ShowDialog();
        }

        private void MockDialog_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Preferences saved for this prototype session. No persistent changes were made.", "StayWise prototype", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public sealed class PrototypeData
    {
        public ObservableCollection<BookingPreview> RecentBookings { get; } = new()
        {
            new("BK-2048", "Amelia Rodriguez", "304", "12 Jun", "15 Jun", "2", "Confirmed"),
            new("BK-2047", "Marcus Chen", "512", "12 Jun", "14 Jun", "1", "Confirmed"),
            new("BK-2046", "Sofia Williams", "207", "13 Jun", "17 Jun", "3", "Pending"),
            new("BK-2045", "Noah Thompson", "118", "14 Jun", "16 Jun", "2", "Confirmed")
        };

        public ObservableCollection<BookingPreview> AllBookings { get; }

        public ObservableCollection<RoomModel> Rooms { get; } = new()
        {
            new("401", "4", "Standard King", "2", 145, "Ready"),
            new("402", "4", "Deluxe Twin", "3", 205, "Occupied"),
            new("403", "4", "Standard Queen", "2", 125, "Needs Cleaning"),
            new("501", "5", "Executive Suite", "4", 390, "Ready"),
            new("502", "5", "Deluxe King", "2", 240, "Maintenance")
        };

        public ObservableCollection<string> Floors { get; } = new() { "4", "5" };
        public ObservableCollection<RoomTypeGroup> RoomGroups { get; } = new();

        public ObservableCollection<EmployeeModel> Employees { get; } = new()
        {
            new("EMP-1042", "Ava Richardson", "555-0182", "ava@staywise.com", "Receptionist", "Active"),
            new("EMP-1038", "Daniel Morgan", "555-0189", "daniel@staywise.com", "Housekeeping", "Active"),
            new("EMP-1007", "Sofia Patel", "555-0117", "sofia@staywise.com", "Manager", "Active")
        };

        public ObservableCollection<AccountModel> Accounts { get; } = new()
        {
            new("ACC-001", "jordan.smith", "Jordan Smith", "Admin", "Active"),
            new("ACC-002", "ava.richardson", "Ava Richardson", "Receptionist", "Active"),
            new("ACC-003", "sofia.patel", "Sofia Patel", "Manager", "Active")
        };

        public PrototypeData()
        {
            AllBookings = new ObservableCollection<BookingPreview>(RecentBookings)
            {
                new("BK-2044", "Olivia Martin", "401", "15 Jun", "18 Jun", "2", "Confirmed"),
                new("BK-2043", "Ethan Brown", "220", "16 Jun", "19 Jun", "4", "Pending")
            };
            RebuildRoomGroups();
        }

        public void RebuildRoomGroups()
        {
            RoomGroups.Clear();
            foreach (var group in Rooms.OrderBy(room => room.Type).ThenBy(room => room.Number).GroupBy(room => room.Type))
                RoomGroups.Add(new RoomTypeGroup(group.Key, group.ToList()));
        }
    }

    public sealed record BookingPreview(string Reference, string Guest, string Room, string CheckIn, string CheckOut, string Guests, string Status);
    public sealed record RoomModel(string Number, string Floor, string Type, string Capacity, decimal Price, string Status);
    public sealed record EmployeeModel(string Id, string Name, string Phone, string Email, string Position, string Status);
    public sealed record AccountModel(string Id, string Username, string Employee, string Role, string Status);
    public sealed class RoomTypeGroup
    {
        public string Title { get; }
        public IReadOnlyList<RoomModel> Rooms { get; }
        public int Count => Rooms.Count;
        public string CountLabel => $"{Count} room{(Count == 1 ? "" : "s")}";
        public RoomTypeGroup(string title, IReadOnlyList<RoomModel> rooms) { Title = title; Rooms = rooms; }
    }
}
