namespace FrisörenSörenAPI.Dto
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<BookingDto> Bookings { get; set; }
    }
}
