using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingResponse>> GetAllAsync(int? showId = null, string? status = null);
    Task<BookingResponse> GetByIdAsync(int id);
    Task<IEnumerable<BookingResponse>> GetMineAsync(int userId);
    Task<BookingResponse> GetMineByIdAsync(int id, int userId);
    Task<IEnumerable<BookingResponse>> GetByShowIdAsync(int showId);
    Task<BookingResponse> CreateAsync(CreateBookingRequest request, int userId);
    Task<BookingResponse> CancelAsync(int id);
    Task<BookingResponse> CancelMineAsync(int id, int userId);
}
