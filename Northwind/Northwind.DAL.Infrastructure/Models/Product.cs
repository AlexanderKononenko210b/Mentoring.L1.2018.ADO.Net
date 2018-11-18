namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a model <see cref="Product"/> class.
    /// </summary>
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }
    }
}
