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
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }

                var booking = await _bookingService.GetBookingById(id);
                if (booking == null)
                {
                    return NotFound();
                }
                return Ok(booking);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }
        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }

                var customer = await _customerService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }

                var customers = await _customerService.GetAllCustomers();  // Anropa asynkron metod
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);  // Kartlägg resultatet till CustomerDto

                return Ok(customerDtos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("BookingsThisWeek")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersWithBookingsThisWeek()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }
                var customers = await _customerService.GetCustomersWithBookingsThisWeek();

                // Kontrollera om listan med kunder är tom
                if (customers.Count() == 0)
                {
                    return Ok("No Bookings this week");
                }

                return Ok(customers);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }
        [HttpGet("CustomerNumberOfBookingsSpecificWeek/{customerId}/{weekNumber}")]
        public async Task<ActionResult<int>> GetBookingCountForCustomerInWeek(int customerId, int weekNumber)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }
        [HttpGet("GetAllBookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userRole = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }
                if (userRole != "Company")
                {
                    return Unauthorized("User does not have access.");
                }
                var bookings = await _bookingService.GetAllBookings();
                return Ok(bookings);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }


        [HttpPost("CompanyAddBooking/{customerId}")]
        public async Task<ActionResult<Booking>> CompanyAddBooking(int customerId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var addedBooking = await _bookingService.CompanyAddBooking(customerId, bookingDto, userEmail);
                return CreatedAtAction(nameof(GetBookingById), new { id = addedBooking.BookingId }, addedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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



        [HttpPut("CompanyUpdate/{bookingId}")]
        public async Task<ActionResult<Booking>> CompanyUpdateBooking(int bookingId, [FromBody] BookingDto bookingDto)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var updatedBooking = await _bookingService.CompanyUpdateBooking(bookingId, bookingDto, userEmail);
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

        [HttpDelete("CompanyDelete/{bookingId}")]
        public async Task<ActionResult> CompanyDeleteBooking(int bookingId)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User not logged in.");
                }

                var result = await _bookingService.CompanyDeleteBooking(bookingId, userEmail);
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
    }
}

