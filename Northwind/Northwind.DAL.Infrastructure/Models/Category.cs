namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a model <see cref="Category"/> class.
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public byte[] Picture { get; set; }
    }
}
