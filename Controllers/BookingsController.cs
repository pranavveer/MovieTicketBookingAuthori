using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Authentication;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _svc;
    public BookingsController(IBookingService svc) => _svc = svc;

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id : throw new UnauthorizedAccessException("User ID claim is missing.");

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAll(
        [FromQuery] int? showId, [FromQuery] string? status)
        => Ok(await _svc.GetAllAsync(showId, status));

    [HttpGet("mine")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetMine()
        => Ok(await _svc.GetMineAsync(CurrentUserId));

    [HttpGet("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<BookingResponse>> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpGet("{id:int}/mine")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> GetMineById(int id)
        => Ok(await _svc.GetMineByIdAsync(id, CurrentUserId));

    [HttpGet("show/{showId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByShow(int showId)
        => Ok(await _svc.GetByShowIdAsync(showId));

    [HttpPost]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest req)
    {
        var created = await _svc.CreateAsync(req, CurrentUserId);
        return CreatedAtAction(nameof(GetMineById), new { id = created.BookingId }, created);
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<BookingResponse>> CancelAsAdmin(int id)
        => Ok(await _svc.CancelAsync(id));

    [HttpPut("{id:int}/cancel-mine")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> CancelMine(int id)
        => Ok(await _svc.CancelMineAsync(id, CurrentUserId));
}
