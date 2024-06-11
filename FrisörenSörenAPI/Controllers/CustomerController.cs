using AutoMapper;
using FrisörenSörenAPI.Dto;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenAPI.Repositories;
using FrisörenSörenModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrisörenSörenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public CustomerController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }
        [HttpPost("Customer/{customerId}/addBooking")]
        public async Task<ActionResult<BookingDto>> CustomerAddBooking(int customerId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var addedBooking = await _bookingService.CustomerAddBooking(customerId, bookingDto, userEmail);
                return CreatedAtAction(nameof(GetBookingById), new { id = addedBooking.BookingId }, addedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("CustomerUpdate/{bookingId}")]
        public async Task<ActionResult<BookingDto>> CustomerUpdateBooking(int bookingId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var updatedBooking = await _bookingService.CustomerUpdateBooking(bookingId, bookingDto, userEmail);
                return Ok(updatedBooking);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("CustomerDelete/{bookingId}")]
        public async Task<ActionResult> CustomerDeleteBooking(int bookingId)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var result = await _bookingService.CustomerDeleteBooking(bookingId, userEmail);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
    }
}
