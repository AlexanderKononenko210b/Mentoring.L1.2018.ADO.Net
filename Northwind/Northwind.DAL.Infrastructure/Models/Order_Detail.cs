namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a model <see cref="Order_Detail"/> class.
    /// </summary>
    public class Order_Detail
    {
        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }
    }
}
