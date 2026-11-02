namespace MovieTicketBooking.Authentication;

public interface IJwtService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(
        int userId, string username, string email, string role);
}
