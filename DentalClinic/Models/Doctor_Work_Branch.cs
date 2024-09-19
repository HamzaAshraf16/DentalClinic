using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace DentalClinic.Models
{
    [PrimaryKey(nameof(DoctorWorkBranchId),nameof(DoctorID),nameof(BranchID))]
    public class Doctor_Work_Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorWorkBranchId { get; set; }
        public Doctor_Work_Branch()
        {
            _startTime = new TimeSpan(13, 0, 0);
            _endTime = new TimeSpan(1, 0, 0);
            _isWork=true;
        }
        

        private string _day;

        [RegularExpression(@"^(الأثنين|الثلاثاء|الأربعاء|الخميس|الجمعة|السبت)$",
        ErrorMessage = "يجب ان يكون يوم عمل ")]
        public string Day { get { return _day; } set { _day = value; } }


        private bool _isWork;
        [DefaultValue(true)]
        public bool IsWork {get { return _isWork;} set { _isWork = value; } }

        private TimeSpan _startTime;
        [Column(TypeName = "time")]
        public TimeSpan StartTime { get { return _startTime; } set { _startTime = value; } }
        
        
        private TimeSpan _endTime;
        
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get { return _endTime; } set { _endTime = value; } }


        [ForeignKey("Branch")]
        public int BranchID { get; set; }

        public virtual Branch Branch { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }

        public virtual Doctor Doctor { get; set; }



    }
}
