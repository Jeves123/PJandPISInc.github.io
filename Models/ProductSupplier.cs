using System.ComponentModel.DataAnnotations.Schema;

namespace PJ_P_Installation_Management_System.Models
{
    public class ProductSupplier
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
