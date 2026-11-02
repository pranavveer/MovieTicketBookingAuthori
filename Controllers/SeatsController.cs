using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Authentication;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _svc;
    public SeatsController(ISeatService svc) => _svc = svc;

    [HttpGet("screen/{screenId:int}")]
    public async Task<ActionResult<IEnumerable<SeatResponse>>> GetByScreen(int screenId) => Ok(await _svc.GetByScreenIdAsync(screenId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SeatResponse>> GetById(int id) => Ok(await _svc.GetByIdAsync(id));

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<SeatResponse>> Create([FromBody] CreateSeatRequest req)
    {
        var created = await _svc.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.SeatId }, created);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<SeatResponse>>> BulkCreate([FromBody] BulkCreateSeatsRequest req)
        => StatusCode(StatusCodes.Status201Created, await _svc.BulkCreateAsync(req));

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SeatResponse>> Update(int id, [FromBody] UpdateSeatRequest req) => Ok(await _svc.UpdateAsync(id, req));

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await _svc.DeleteAsync(id); return NoContent(); }
}
