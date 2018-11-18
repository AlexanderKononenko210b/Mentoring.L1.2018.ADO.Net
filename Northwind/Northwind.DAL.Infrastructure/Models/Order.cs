using System;
using System.Collections.Generic;
using Northwind.DAL.Infrastructure.Enums;

namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a <see cref="Order"/> class.
    /// </summary>
    public class Order
    {
        public int OrderID { get; set; }

        public string CustomerID { get; set; }

        public int EmployeeID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        public decimal? Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public IList<Order_Detail> OrderDetails { get; set; }

        public IList<Product> Products { get; set; }

        public Status Status { get; set; }
    }
}
