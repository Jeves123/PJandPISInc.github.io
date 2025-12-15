namespace PJ_P_Installation_Management_System.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }

        public ICollection<ScheduleStaff> ScheduleAssignments { get; set; } = new List<ScheduleStaff>();
    }

}
