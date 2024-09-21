namespace DentalClinic.DTO
{
    public class DoctorWorkBranchDTO
    {
        public string Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string BranchName { get; set; }
    }
}
