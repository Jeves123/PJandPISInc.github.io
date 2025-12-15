using System.ComponentModel.DataAnnotations;

namespace PJ_P_Installation_Management_System.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemId { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Delivered quantity cannot be negative")]
        public int DeliveredQuantity { get; set; }
        public int Stack { get; set; } = 0;

        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
    }
}