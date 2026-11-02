using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);
    Task<AuthResponseDto> LoginAsync(LoginDto request);
}
