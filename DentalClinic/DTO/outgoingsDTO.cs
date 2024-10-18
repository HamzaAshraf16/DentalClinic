namespace DentalClinic.DTO
{
    public class outgoingsDTO
    {
        public int outgoingsId { get; set; }
        public DateTime Date { get; set; }
        public string NameOfOutgoings { get; set; }
        public int Cost { get; set; }
        public string BranchName { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public int BranchID { get; set; }
    }

}
