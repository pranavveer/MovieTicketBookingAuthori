using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Auth;

public sealed class RegisterDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(15, MinimumLength = 7)]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(8), StringLength(100)]
    public string Password { get; set; } = string.Empty;
}
