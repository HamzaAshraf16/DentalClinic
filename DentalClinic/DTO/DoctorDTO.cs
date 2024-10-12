namespace DentalClinic.DTO
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? UserId { get; set; }
        public ICollection<DoctorWorkBranchDTO> WorkBranches { get; set; }
    }
}
