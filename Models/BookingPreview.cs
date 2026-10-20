
namespace HotelManagementSystem.Models;

public sealed record BookingPreview(
    string Reference,
    string Guest,
    string Room,
    DateTime CheckIn,
    DateTime CheckOut,
    int Guests,
    string Status);
