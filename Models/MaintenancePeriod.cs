namespace HotelManagementSystem.Models;

public sealed record MaintenancePeriod(
    string RoomNumber,
    DateTime StartDate,
    DateTime EndDate);
