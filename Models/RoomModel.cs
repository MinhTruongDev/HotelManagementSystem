
namespace HotelManagementSystem.Models;

public sealed record RoomModel(
    string Number,
    string Floor,
    string Type,
    string Capacity,
    decimal Price,
    string Status,
    DateTime? OccupiedUntil = null);
