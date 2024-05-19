using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrisörenSörenModels
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public int? CustomerId { get; set; }
        public int? CompanyId { get; set; }
    }
}
