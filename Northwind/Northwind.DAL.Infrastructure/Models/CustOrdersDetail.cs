namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a model <see cref="CustOrdersDetail"/> class.
    /// </summary>
    public class CustOrdersDetail
    {
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public int Discount { get; set; }

        public decimal ExtendedPrice { get; set; }
    }
}
