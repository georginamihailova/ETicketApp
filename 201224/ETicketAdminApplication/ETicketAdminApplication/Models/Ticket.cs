using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicketAdminApplication.Models
{
    public class Ticket
    {
 
        public Guid Id { get; set; }

        public string TicketName { get; set; }
    
        public string TicketImage { get; set; }

        public string TicketDescription { get; set; }
  
        public double TicketPrice { get; set; }

        public double TicketRating { get; set; }
    }
}
