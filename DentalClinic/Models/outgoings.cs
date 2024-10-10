using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.Models
{
    [Table("outgoings")]
    public class outgoings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int outgoingsId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string NameOfOutgoings { get; set; }

        private int _cost;
        [Required]
        public int Cost
        {
            get
            { return _cost; }
            set
            {
                if (value > 0)
                    _cost = value;
                else
                    _cost = 0;

            }
        }
        private DateTime _date;
        [Required]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
            }
        }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("Branch")]
        public int BranchID { get; set; }

        public virtual Branch Branch { get; set; }
    }
}
