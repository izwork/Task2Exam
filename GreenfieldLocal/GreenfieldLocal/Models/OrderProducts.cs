namespace GreenfieldLocal.Models
{
    public class OrderProducts
    {
        public int OrderProductsId { get; set; }
        public int ProductsId { get; set; } // Links to an order. 
        public int OrdersId { get; set; } // Links to an order.
        public int Quantity { get; set; } // Tracks the quantity of items within the order. 

        public Products Products { get; set; } 
        public Orders Orders { get; set; }
    }
}
