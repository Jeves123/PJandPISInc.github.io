using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJ_P_Installation_Management_System.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProductSupplier> ProductSuppliers { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }

}