﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicket.Domain.DomainModels
{
    public class TicketInShoppingCart : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public Guid ShoppingCartId { get; set; }
        public ShoppingCart UserCart { get; set; }

        public int Quantity { get; set; }
    }
}
