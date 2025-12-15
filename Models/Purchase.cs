using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJ_P_Installation_Management_System.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }

        public string OrderId { get; set; }

        public int SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string OrderNumber { get; set; }

        public string DeliveryTrackingNumber { get; set; }

        [Display(Name = "Purchase Order No.")]
        public string PurchaseOrderNo { get; set; }

        public Supplier Supplier { get; set; }

        public bool IsCompleted { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    }

}