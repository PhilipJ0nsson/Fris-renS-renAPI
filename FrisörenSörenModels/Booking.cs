using System.ComponentModel.DataAnnotations;

namespace FrisörenSörenModels
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
