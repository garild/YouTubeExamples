using HotelBooking.Application.Bookings.Commands;
using HotelBooking.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISender _sender;

    public BookingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookRoomCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BookingOverlapException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
