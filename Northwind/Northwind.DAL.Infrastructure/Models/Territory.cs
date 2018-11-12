namespace Northwind.DAL.Infrastructure.Models
{
    /// <summary>
    /// Represents a model <see cref="Territory"/> class.
    /// </summary>
    public class Territory
    {
        public string TerritoryID { get; set; }

        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }

        public Region Region { get; set; }
    }
}
