﻿using ETicket.Domain.DomainModels;
using ETicket.Domain.DTO;
using ETicket.Repository.Interface;
using ETicket.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETicket.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        //private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            //_mailRepository = mailRepository;
        }


        public bool deleteTicketFromShoppingCart(string userId, Guid ticketId)
        {
            if (!string.IsNullOrEmpty(userId) && ticketId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(ticketId)).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userCard = loggedInUser.UserCart;

                var allTickets = userCard.TicketInShoppingCarts.ToList();

                var allTicketPrices = allTickets.Select(z => new
                {
                    ProductPrice = z.Ticket.TicketPrice,
                    Quantity = z.Quantity
                }).ToList();

                double totalPrice = 0.0;

                foreach (var item in allTicketPrices)
                {
                    totalPrice += item.Quantity * item.ProductPrice;
                }

                var reuslt = new ShoppingCartDto
                {
                    Tickets = allTickets,
                    TotalPrice = totalPrice
                };

                return reuslt;
            }
            return new ShoppingCartDto();
        }

        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCard = loggedInUser.UserCart;

                //EmailMessage mail = new EmailMessage();
                //mail.MailTo = loggedInUser.Email;
                //mail.Subject = "Sucessfuly created order!";
                //mail.Status = false;


                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);

                List<TicketInOrder> ticketsInOrders = new List<TicketInOrder>();

                var result = userCard.TicketInShoppingCarts.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order,
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0.0;

                sb.AppendLine("Your order is completed. The order conatins: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var currentItem = result[i - 1];
                    totalPrice += currentItem.Quantity * currentItem.SelectedTicket.TicketPrice;
                    sb.AppendLine(i.ToString() + ". " + currentItem.SelectedTicket.TicketName+ " with quantity of: " + currentItem.Quantity + " and price of: $" + currentItem.SelectedTicket.TicketPrice);
                }

                sb.AppendLine("Total price for your order: " + totalPrice.ToString());

                //mail.Content = sb.ToString();


                ticketsInOrders.AddRange(result);

                foreach (var item in ticketsInOrders)
                {
                    this._ticketInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.TicketInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);
                //this._mailRepository.Insert(mail);

                return true;
            }

            return false;
        }
    }
}
