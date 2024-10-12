namespace DentalClinic.DTO
{
    public class outgoingsDTO
    {
        public DateTime Date { get; set; }
        public string NameOfOutgoings { get; set; }
        public int Cost { get; set; }
        public int BranchID { get; set; }
        public int DoctorId { get; set; }
    }
}
