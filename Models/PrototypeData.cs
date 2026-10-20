using System.Collections.ObjectModel;

namespace HotelManagementSystem.Models;

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