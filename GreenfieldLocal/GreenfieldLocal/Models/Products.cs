namespace GreenfieldLocal.Models
{
    public class Products
    {
        public int ProductsId { get; set; }
        public int SuppliersId { get; set; } // Links each product to a supplier
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public Suppliers? Suppliers { get; set; } // Navigation property for suppliers. 

        public ICollection<OrderProducts>? OrderProducts { get; set; }
        public ICollection<BasketProducts>? BasketProducts { get; set; }
    }
}
