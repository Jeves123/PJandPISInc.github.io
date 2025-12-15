// ViewModels/PurchaseCreateViewModel.cs
using PJ_P_Installation_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class PurchaseItemViewModel
{
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}

public class PurchaseCreateViewModel
{
    [Required]
    public int SupplierId { get; set; }

    [Required]
    public DateTime PurchaseDate { get; set; }

    [Required]
    public string Status { get; set; }

    public List<PurchaseItemViewModel> Items { get; set; } = new();
}

