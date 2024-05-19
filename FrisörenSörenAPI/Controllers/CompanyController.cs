using AutoMapper;
using FrisörenSörenAPI.Dto;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenAPI.Repositories;
using FrisörenSörenModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrisörenSörenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public CompanyController(IBookingService bookingService, ICustomerService customerService, IMapper mapper)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet("GetBookingById{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomers();  // Anropa asynkron metod
            var customerDtos = _mapper.Map<List<CustomerDto>>(customers);  // Kartlägg resultatet till CustomerDto

            return Ok(customerDtos);
        }

        [HttpGet("BookingsThisWeek")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersWithBookingsThisWeek()
        {
            var customers = await _customerService.GetCustomersWithBookingsThisWeek();

            // Kontrollera om listan med kunder är tom
            if (customers.Count() == 0)
            {
                return Ok("No Bookings this week");
            }

            return Ok(customers);
        }
        [HttpGet("CustomerNumberOfBookingsSpecificWeek/{customerId}/{weekNumber}")]
        public async Task<ActionResult<int>> GetBookingCountForCustomerInWeek(int customerId, int weekNumber)
        {
            try
            {
                var bookingCount = await _customerService.GetBookingCountForCustomerInWeek(customerId, weekNumber);
                return Ok(bookingCount);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // Returnera NotFound om kunden inte hittades
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Hantera andra fel med intern serverfelkod och meddelande
            }
        }
        [HttpGet("GetAllBookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookings();
            return Ok(bookings);
        }


        [HttpPost("CompanyAddBooking/{customerId}")]
        public async Task<ActionResult<Booking>> CompanyAddBooking(int customerId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var addedBooking = await _bookingService.CompanyAddBooking(customerId, bookingDto);
                return CreatedAtAction(nameof(GetBookingById), new { id = addedBooking.BookingId }, addedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("CompanyUpdate/{bookingId}")]
        public async Task<ActionResult<Booking>> CompanyUpdateBooking(int bookingId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var updatedBookingFull = await _bookingService.CompanyUpdateBooking(bookingId, bookingDto);
                return Ok(updatedBookingFull);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("CompanyDelete")]
        public async Task<ActionResult> CompanyDeleteBooking(int bookingId)
        {
            var result = await _bookingService.CompanyDeleteBooking(bookingId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

