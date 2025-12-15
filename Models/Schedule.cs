using System.ComponentModel.DataAnnotations;

namespace PJ_P_Installation_Management_System.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public DateTime ScheduledDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string? TaskDescription { get; set; }  // ⬅ nullable
        public string? Position { get; set; }         // ⬅ nullable
        public string? Location { get; set; }         // ⬅ nullable

        // Foreign key to CustomerPurchase
        public int? CustomerPurchaseId { get; set; }  // ⬅ nullable if optional
        public CustomerPurchase? CustomerPurchase { get; set; }
        public ICollection<ScheduleStaff> StaffAssignments { get; set; } = new List<ScheduleStaff>();

    }
}
