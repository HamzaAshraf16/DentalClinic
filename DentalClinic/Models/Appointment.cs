using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinic.Models
{
    [Table("Appointment")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }


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

        private TimeSpan _time;
        [Required]
        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                _time = value;
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

        private string _report;
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Reports
        {
            get { return _report; }
            set
            {
                    _report = value;
            }
        }


        private int _type;
        [Required]
        [Range(0, 2)]// كشف=0   , اعاده=1 , جلسه=2
        public int Type
        {
            get { return _type; }
            set
            {
                if (value >= 0 && value <= 2)
                    _type = value;
                else
                    _type = 0;
            }
        }


        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

    }
}
