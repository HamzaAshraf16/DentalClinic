namespace DentalClinic.DTO
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? Age { get; set; }
        public string? UserId { get; set; }
        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool StomachAche { get; set; }
        public bool PeriodontalDisease { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsBreastfeeding { get; set; }
        public bool IsSmoking { get; set; }
        public bool KidneyDiseases { get; set; }
        public bool HeartDiseases { get; set; }

    }
}
