namespace PJ_P_Installation_Management_System.Models
{
    public class ScheduleStaff
    {
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }
    }
}
