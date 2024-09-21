using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DentalClinic.Models
{
    public class PatientHistory
    {
        [Key]
        public int PatientHistoryID { get; set; }

        [DefaultValue(false)]
        public bool Hypertension { get; set; }

        [DefaultValue(false)]
        public bool Diabetes { get; set; }

        [Column("Stomach_Ache")]
        [DefaultValue(false)]
        public bool StomachAche { get; set; }

        [Column("periodontal_Disease")]
        [DefaultValue(false)]
        public bool PeriodontalDisease { get; set; }

        [Column("IS_Pregnant")]
        [DefaultValue(false)]
        public bool IsPregnant { get; set; }

        [Column("IS_Breastfeeding")]
        [DefaultValue(false)]
        public bool IsBreastfeeding { get; set; }

        [Column("IS_Smoking")]
        [DefaultValue(false)]
        public bool IsSmoking { get; set; }

        [Column("Kidney_diseases")]
        [DefaultValue(false)]
        public bool KidneyDiseases { get; set; }

        [Column("Heart_Diseases")]
        [DefaultValue(false)]
        public bool HeartDiseases { get; set; }
        public virtual Patient? Patient { get; set; }

    }
}
