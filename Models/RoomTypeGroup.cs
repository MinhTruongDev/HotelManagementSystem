namespace HotelManagementSystem.Models;


public sealed class RoomTypeGroup
{
    public string Title { get; }
    public IReadOnlyList<RoomModel> Rooms { get; }
    public int Count => Rooms.Count;
    public string CountLabel => $"{Count} room{(Count == 1 ? "" : "s")}";
    public RoomTypeGroup(string title, IReadOnlyList<RoomModel> rooms) { Title = title; Rooms = rooms; }
}
