using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings;

public class CreateBookingRequest
{
    [Required]
    public int ShowId { get; set; }

    [Required, MinLength(1)]
    public List<int> SeatIds { get; set; } = new();
}
