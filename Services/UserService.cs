using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Authentication;
using MovieTicketBooking.Data;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Exceptions;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public sealed class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(ApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Role = UserRoles.Customer,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null)
            throw new BadRequestException("Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException("Invalid email or password.");

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _context.SaveChangesAsync();
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var generated = _jwtService.GenerateToken(
            user.UserId, user.Username, user.Email, user.Role);

        return new AuthResponseDto
        {
            Token = generated.Token,
            ExpiresAtUtc = generated.ExpiresAtUtc,
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}
