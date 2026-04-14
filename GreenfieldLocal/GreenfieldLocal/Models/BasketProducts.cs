namespace GreenfieldLocal.Models
{
    public class BasketProducts
    {
        public int BasketProductsId { get; set; }
        public int BasketId { get; set; } // Links to a basket 
        public int ProductId { get; set; } // Links to a product
        public int Quantity { get; set; }
        public Products Products { get; set; }
        public Basket Basket { get; set; }
    }
}
