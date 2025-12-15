using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PJ_P_Installation_Management_System.Models
{
    public class CustomerPurchase
    {
        public int CustomerPurchaseId { get; set; }

        [Display(Name = "Purchase Order")]
        public string OrderId { get; set; }

        [Required]
        [Display(Name = "Customer / Project")]
        public string CustomerProject { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Installation Status")]
        public string InstallationStatus { get; set; } = "Pending";

        public string? InstallationLocation { get; set; }
        public DateTime? InstallationDate { get; set; }
        
        public DateTime? InstallationEndDate { get; set; }

        public decimal? AmountReceived { get; set; }      


        // Payment status (Pending / Completed)
        public string? PaymentStatus { get; set; } = "Pending";

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        public decimal? LaborFeeAmount { get; set; }

        public decimal? GrandTotal { get; set; }

        public decimal? ChangeAmount { get; set; }


        // Navigation
        public ICollection<CustomerPurchaseItem> CustomerPurchaseItems { get; set; } = new List<CustomerPurchaseItem>();
        public ICollection<Schedule> Schedules { get; set; }

        public CustomerPurchase()
        {
            OrderId = "ORD-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        }

    }

}
