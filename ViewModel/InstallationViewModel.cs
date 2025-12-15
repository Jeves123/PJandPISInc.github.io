namespace PJ_P_Installation_Management_System.ViewModel
{
    public class InstallationViewModel
    {
        public int CustomerPurchaseId { get; set; }   // For linking action button
        public string OrderId { get; set; }
        public string CustomerProject { get; set; }
        public string InstallationStatus { get; set; } // Added to track installation
        public string Status { get; set; }             // Optional: overall purchase status
        public DateTime? InstallationDate { get; set; }
        public DateTime? InstallationEndDate { get; set; }
    }
}
