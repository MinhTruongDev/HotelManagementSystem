using System.Collections.ObjectModel;

namespace HotelManagementSystem.Models;

public sealed class PrototypeData
{
public PrototypeData()
{
    var today = DateTime.Today;

    RecentBookings = new ObservableCollection<BookingPreview>
    {
        new("BK-2048", "Amelia Rodriguez", "304",
            today.AddDays(1), today.AddDays(4), 2, "Confirmed"),

        new("BK-2047", "Marcus Chen", "512",
            today.AddDays(1), today.AddDays(3), 1, "Confirmed"),

        new("BK-2046", "Sofia Williams", "207",
            today.AddDays(2), today.AddDays(6), 3, "Pending"),

        new("BK-2045", "Noah Thompson", "118",
            today.AddDays(3), today.AddDays(5), 2, "Confirmed")
    };

    AllBookings = new ObservableCollection<BookingPreview>(
        RecentBookings.Concat(new[]
        {
            new BookingPreview(
                "BK-2044", "Olivia Martin", "401",
                today.AddDays(4), today.AddDays(7), 2, "Confirmed"),

            new BookingPreview(
                "BK-2043", "Ethan Brown", "220",
                today.AddDays(5), today.AddDays(8), 4, "Pending")
        }));

    RebuildRoomGroups();
}
    public ObservableCollection<BookingPreview> RecentBookings { get; } 
    public ObservableCollection<BookingPreview> AllBookings { get; }
    public ObservableCollection<RoomModel> Rooms { get; } = new()
    {
        new("401", "4", "Standard King", "2", 145, "Ready"),
        new("402", "4", "Deluxe Twin", "3", 205, "Occupied", DateTime.Today.AddDays(2)),
        new("403", "4", "Standard Queen", "2", 125, "Needs Cleaning"),
        new("501", "5", "Executive Suite", "4", 390, "Ready"),
        new("502", "5", "Deluxe King", "2", 240, "Maintenance")
    };
    public ObservableCollection<MaintenancePeriod> MaintenancePeriods { get; } = new();
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

    public void RebuildRoomGroups()
    {
        RoomGroups.Clear();
        foreach (var group in Rooms.OrderBy(room => room.Type).ThenBy(room => room.Number).GroupBy(room => room.Type))
            RoomGroups.Add(new RoomTypeGroup(group.Key, group.ToList()));
    }
}