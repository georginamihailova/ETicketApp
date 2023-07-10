using ETicket.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETicket.Repository.Interface
{
    public interface IOrderRepository
    {
        public List<Order> getAllOrders();
        public Order getOrderDetails(BaseEntity model);
    }
}
