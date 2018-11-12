using System;
using System.Collections.Generic;
using Northwind.DAL.Infrastructure.Models;

namespace Northwind.DAL.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents a <see cref="IOrderRepository"/> interface.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{Order}"/></returns>
        IEnumerable<Order> GetOrders();

        /// <summary>
        /// Get order detail by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Order"/></returns>
        Order GetOrderDetailById(int id);

        /// <summary>
        /// Add new order.
        /// </summary>
        /// <param name="order">The <see cref="Order"/></param>
        /// <returns>The <see cref="Order"/></returns>
        Order AddOrder(Order order);

        /// <summary>
        /// Set order status to InWork
        /// </summary>
        /// <param name="id">The order id.</param>
        /// <param name="orderDate">The order date.</param>
        /// <param name="requiredDate">The required date.</param>
        /// <returns>The <see cref="Order"/></returns>
        Order SetOrderStatusInWork(int id, DateTime orderDate, DateTime requiredDate);

        /// <summary>
        /// Transfer Order To Finished status.
        /// </summary>
        /// <param name="id">The order id.</param>
        /// <param name="shippedDate">The shipped date.</param>
        /// <returns>The <see cref="Order"/></returns>
        Order SetOrderStatusFinished(int id, DateTime shippedDate);

        /// <summary>
        /// Update order.
        /// </summary>
        /// <param name="newOrder">The modified order.</param>
        /// <returns>The <see cref="Order"/></returns>
        Order UpdateOrder(Order newOrder);

        /// <summary>
        /// Delete order.
        /// </summary>
        /// <param name="id">The order id.</param>
        /// <returns>The <see cref="Order"/></returns>
        Order DeleteOrder(int id);

        /// <summary>
        /// Get cust order hist.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The <see cref="IEnumerable{CustOrderHist}"/></returns>
        IEnumerable<CustOrderHist> GetCustOrderHist(string customerId);

        /// <summary>
        /// Get cust orders detail.
        /// </summary>
        /// <param name="orderId">The order detail.</param>
        /// <returns>The <see cref="IEnumerable{CustOrdersDetail}"/></returns>
        IEnumerable<CustOrdersDetail> GetCustOrdersDetail(int orderId);
    }
}
