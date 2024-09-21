namespace DentalClinic.DTO
{
    public class PatientHistoryDto
    {
        public int PatientHistoryID { get; set; }
        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool StomachAche { get; set; }
        public bool PeriodontalDisease { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsBreastfeeding { get; set; }
        public bool IsSmoking { get; set; }
        public bool KidneyDiseases { get; set; }
        public bool HeartDiseases { get; set; }
        public PatientForHistoryDto Patient { get; set; }
    }
}
