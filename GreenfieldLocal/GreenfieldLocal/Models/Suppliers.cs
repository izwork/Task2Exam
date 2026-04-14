namespace GreenfieldLocal.Models
{
    public class Suppliers
    {
        public int SuppliersId { get; set; }
        public string UserId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierInformation { get; set; }

        public ICollection<Products>? Products { get; set; }
    }
}
