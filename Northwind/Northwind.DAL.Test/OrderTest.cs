using System;
using System.Configuration;
using System.Linq;
using Northwind.DAL.Infrastructure.Models;
using Northwind.DAL.Repositories;
using NUnit.Framework;

namespace Northwind.DAL.Test
{
    /// <summary>
    /// Tests order repository.
    /// </summary>
    [TestFixture]
    public class OrderTest
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["NorthwindConection"].ConnectionString;
        private readonly string _provider = ConfigurationManager.ConnectionStrings["NorthwindConection"].ProviderName;

        private OrderRepository _orderRepository;
        private Order _orderNew;
        private Order _orderModify;

        [SetUp]
        public void Initialize()
        {
            _orderRepository = new OrderRepository(_connectionString, _provider);

            _orderNew = new Order
            {
                CustomerID = "BONAP",
                EmployeeID = 9,
                ShipVia = 1,
                Freight = new decimal(10.19),
                ShipName = "Vins et alcools Chevalier",
                ShipAddress = "2, rue du Commerce",
                ShipCity = "Lyon",
                ShipPostalCode = "69004",
                ShipRegion = string.Empty,
                ShipCountry = "France"
            };

            _orderModify = new Order
            {
                CustomerID = "VINET",
                EmployeeID = 5,
                ShipVia = 3,
                Freight = new decimal(32.38),
                ShipName = "Toms Spezialitäten",
                ShipAddress = "Luisenstr. 48",
                ShipCity = "Münster",
                ShipPostalCode = "44087",
                ShipRegion = "RJ",
                ShipCountry = "Germany"
            };
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        [Test]
        public void GetOrders_Success()
        {
            var result = _orderRepository.GetOrders();

            Assert.True(result.Any());
        }

        /// <summary>
        /// Get order with detail by id.
        /// </summary>
        [Test]
        public void GetOrderDetailById_OrderID_Success()
        {
            var orderId = 10248;

            var result = _orderRepository.GetOrderDetailById(orderId);

            Assert.AreEqual(orderId, result.OrderID);
            Assert.NotNull(result.OrderDetails);
            Assert.NotNull(result.Products);
            Assert.AreEqual(3, result.OrderDetails.Count);
            Assert.AreEqual(3, result.Products.Count);
            Assert.AreEqual("Queso Cabrales", result.Products[0].ProductName);
            Assert.AreEqual("Singaporean Hokkien Fried Mee", result.Products[1].ProductName);
            Assert.AreEqual("Mozzarella di Giovanni", result.Products[2].ProductName);
        }

        /// <summary>
        /// Add new order.
        /// </summary>
        [Test]
        public void AddOrder_Order_Success()
        {
            var result = _orderRepository.AddOrder(_orderNew);

            Assert.True(result.OrderID > 0);
        }

        /// <summary>
        /// Set order status to InWork.
        /// </summary>
        [Test]
        public void SetOrderStatusInWork_OrderId_OrderDate_RequiredDate()
        {
            var orderDate = new DateTime(2018, 10, 3);
            var requiredDate = new DateTime(2018, 11, 29);

            var addOrderResult = _orderRepository.AddOrder(_orderNew);

            Assert.Null(addOrderResult.OrderDate);
            Assert.Null(addOrderResult.RequiredDate);

            var updateOrderResult =
                _orderRepository.SetOrderStatusInWork(addOrderResult.OrderID, orderDate, requiredDate);

            Assert.AreEqual(updateOrderResult.OrderDate.GetValueOrDefault(), orderDate);
            Assert.AreEqual(updateOrderResult.RequiredDate.GetValueOrDefault(), requiredDate);
        }

        /// <summary>
        /// Set order status to InWork.
        /// </summary>
        [Test]
        public void SetOrderStatusFinished_OrderId_ShippedDate()
        {
            var shippedDate = new DateTime(2018, 11, 15);
            var addOrderResult = _orderRepository.AddOrder(_orderNew);
            var orderInWork = _orderRepository.SetOrderStatusInWork(addOrderResult.OrderID, new DateTime(2018, 10, 1), new DateTime(2018, 11, 29));
            var updateOrderResult = _orderRepository.SetOrderStatusFinished(orderInWork.OrderID, shippedDate);

            Assert.AreEqual(updateOrderResult.ShippedDate.GetValueOrDefault(), shippedDate);
        }

        /// <summary>
        /// Update order success.
        /// </summary>
        [Test]
        public void UpdateOrder_OrderForUpdate_Success()
        {
            var orderForUpdate = _orderRepository.AddOrder(_orderNew);
            _orderModify.OrderID = orderForUpdate.OrderID;
            var orderAfterUpdate = _orderRepository.UpdateOrder(_orderModify);

            Assert.AreEqual(_orderModify.CustomerID, orderAfterUpdate.CustomerID);
            Assert.AreEqual(_orderModify.EmployeeID, orderAfterUpdate.EmployeeID);
            Assert.AreEqual(_orderModify.ShipVia, orderAfterUpdate.ShipVia);
            Assert.AreEqual(_orderModify.Freight, orderAfterUpdate.Freight);
            Assert.AreEqual(_orderModify.ShipName, orderAfterUpdate.ShipName);
            Assert.AreEqual(_orderModify.ShipAddress, orderAfterUpdate.ShipAddress);
            Assert.AreEqual(_orderModify.ShipCity, orderAfterUpdate.ShipCity);
            Assert.AreEqual(_orderModify.ShipPostalCode, orderAfterUpdate.ShipPostalCode);
            Assert.AreEqual(_orderModify.ShipRegion, orderAfterUpdate.ShipRegion);
            Assert.AreEqual(_orderModify.ShipCountry, orderAfterUpdate.ShipCountry);
        }

        /// <summary>
        /// Update order with nonexistent id. Expected null.
        /// </summary>
        [Test]
        public void UpdateOrder_OrderModifyWithNonexistentID_Expected_Null()
        {
            _orderModify.OrderID = -1;
            var orderAfterUpdate = _orderRepository.UpdateOrder(_orderModify);

            Assert.Null(orderAfterUpdate);
        }

        /// <summary>
        /// Update order with status "InWork". Expected null.
        /// </summary>
        [Test]
        public void UpdateOrder_OrderModifyWith_InWork_Status_Expected_Null()
        {
            var orderForUpdate = _orderRepository.AddOrder(_orderNew);
            var orderInWorkStatus = _orderRepository.SetOrderStatusInWork(orderForUpdate.OrderID, new DateTime(2018, 10, 1), new DateTime(2018, 11, 29));
            _orderModify.OrderID = orderInWorkStatus.OrderID;
            var orderAfterUpdate = _orderRepository.UpdateOrder(_orderModify);

            Assert.Null(orderAfterUpdate);
        }

        /// <summary>
        /// Update order with status "Finished". Expected null.
        /// </summary>
        [Test]
        public void UpdateOrder_OrderModifyWith_Finished_Status_Expected_Null()
        {
            var orderForUpdate = _orderRepository.AddOrder(_orderNew);
            var orderInWorkStatus = _orderRepository.SetOrderStatusInWork(orderForUpdate.OrderID, new DateTime(2018, 10, 1), new DateTime(2018, 11, 29));
            var orderFinishedStatus = _orderRepository.SetOrderStatusFinished(orderInWorkStatus.OrderID, new DateTime(2018, 11, 15));
            _orderModify.OrderID = orderFinishedStatus.OrderID;
            var orderAfterUpdate = _orderRepository.UpdateOrder(_orderModify);

            Assert.Null(orderAfterUpdate);
        }

        /// <summary>
        /// Delete order success.
        /// </summary>
        [Test]
        public void DeleteOrder_OrderId_Success()
        {
            var orderForDelete = _orderRepository.AddOrder(_orderNew);
            var orderAfterDelete = _orderRepository.DeleteOrder(orderForDelete.OrderID);

            Assert.AreEqual(_orderNew.OrderID, orderAfterDelete.OrderID);
            Assert.AreEqual(_orderNew.CustomerID, orderAfterDelete.CustomerID);
            Assert.AreEqual(_orderNew.EmployeeID, orderAfterDelete.EmployeeID);
            Assert.AreEqual(_orderNew.ShipVia, orderAfterDelete.ShipVia);
            Assert.AreEqual(_orderNew.Freight, orderAfterDelete.Freight);
            Assert.AreEqual(_orderNew.ShipName, orderAfterDelete.ShipName);
            Assert.AreEqual(_orderNew.ShipAddress, orderAfterDelete.ShipAddress);
            Assert.AreEqual(_orderNew.ShipCity, orderAfterDelete.ShipCity);
            Assert.AreEqual(_orderNew.ShipPostalCode, orderAfterDelete.ShipPostalCode);
            Assert.AreEqual(_orderNew.ShipRegion, orderAfterDelete.ShipRegion);
            Assert.AreEqual(_orderNew.ShipCountry, orderAfterDelete.ShipCountry);
        }

        /// <summary>
        /// Delete order with nonexistent id. Expected null.
        /// </summary>
        [Test]
        public void DeleteOrder_NonexistentOrderID_Expected_Null()
        {
            var nonexistentOrderID = -1;
            var orderAfterDelete = _orderRepository.DeleteOrder(nonexistentOrderID);

            Assert.Null(orderAfterDelete);
        }

        /// <summary>
        /// Delete order with status "Finished". Expected null.
        /// </summary>
        [Test]
        public void DeleteOrder_OrderWith_Finished_Status_Expected_Null()
        {
            var orderForDelete = _orderRepository.AddOrder(_orderNew);
            var orderInWorkStatus = _orderRepository.SetOrderStatusInWork(orderForDelete.OrderID, new DateTime(2018, 10, 1), new DateTime(2018, 11, 29));
            var orderFinishedStatus = _orderRepository.SetOrderStatusFinished(orderInWorkStatus.OrderID, new DateTime(2018, 11, 15));
            var orderAfterUpdate = _orderRepository.DeleteOrder(orderFinishedStatus.OrderID);

            Assert.Null(orderAfterUpdate);
        }

        /// <summary>
        /// Get cust order hist.
        /// </summary>
        [Test]
        public void GetCustOrderHist_CustomerId_IEnumarable_CustOrderHist()
        {
            var customerId = "WELLI";

            var result = _orderRepository.GetCustOrderHist(customerId);

            Assert.True(result.Any());
        }

        /// <summary>
        /// Get cust order hist detail.
        /// </summary>
        [Test]
        public void GetCustOrdersDetail_OrderId_IEnumarable_CustOrdersDetail()
        {
            var orderId = 10298;

            var result = _orderRepository.GetCustOrdersDetail(orderId);

            Assert.True(result.Any());
        }
    }
}
