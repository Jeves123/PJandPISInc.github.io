namespace PJ_P_Installation_Management_System.Models
{
    public class CustomerPurchaseItem
    {
        public int CustomerPurchaseItemId { get; set; }

        public int CustomerPurchaseId { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }    // ✅ store the price at purchase time
    }
}
