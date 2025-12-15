using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJ_P_Installation_Management_System.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "BrandName")]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public string Description { get; set; }

        public string Unit { get; set; }
        public ICollection<ProductSupplier> ProductSuppliers { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }

}