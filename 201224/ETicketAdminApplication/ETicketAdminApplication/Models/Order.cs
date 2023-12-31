﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicketAdminApplication.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public TicketShopApplicationUser User { get; set; }

        public ICollection<TicketInOrder> Tickets { get; set; }
    }
}
