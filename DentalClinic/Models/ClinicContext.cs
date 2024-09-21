using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Models
{
    public class ClinicContext : DbContext
    {
        public ClinicContext()
        {

        }

        public ClinicContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Branch> Branchs { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Doctor_Work_Branch> DoctorWorkBranchs { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientHistory> PatientsHistory { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // تعديل خصائص PatientHistory
            modelBuilder.Entity<PatientHistory>(entity =>
            {
                entity.Property(p => p.Hypertension)
                    .HasDefaultValue(false);

                entity.Property(p => p.Diabetes)
                    .HasDefaultValue(false);

                entity.Property(p => p.StomachAche)
                    .HasDefaultValue(false)
                    .HasColumnName("Stomach_Ache");

                entity.Property(p => p.PeriodontalDisease)
                    .HasDefaultValue(false)
                    .HasColumnName("periodontal_Disease");

                entity.Property(p => p.IsPregnant)
                    .HasDefaultValue(false)
                    .HasColumnName("IS_Pregnant");

                entity.Property(p => p.IsBreastfeeding)
                    .HasDefaultValue(false)
                    .HasColumnName("IS_Breastfeeding");

                entity.Property(p => p.IsSmoking)
                    .HasDefaultValue(false)
                    .HasColumnName("IS_Smoking");

                entity.Property(p => p.KidneyDiseases)
                    .HasDefaultValue(false)
                    .HasColumnName("Kidney_diseases");

                entity.Property(p => p.HeartDiseases)
                    .HasDefaultValue(false)
                    .HasColumnName("Heart_Diseases");
            });
            modelBuilder.Entity<Doctor_Work_Branch>(entity =>
            {
                entity.Property(p => p.IsWork)
                    .HasDefaultValue(true);

                entity.Property(p => p.StartTime)
                    .HasDefaultValue(new TimeSpan(13, 0, 0));

                entity.Property(p => p.EndTime)
                    .HasDefaultValue(new TimeSpan(1, 0, 0));
            });

            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Phonenumber)
                    .IsRequired()
                    .HasColumnType("nchar(11)");

                entity.HasIndex(p => p.Phonenumber)
                    .IsUnique();
            });

            modelBuilder.Entity<Patient>()
            .HasOne(p => p.PatientHistory)
            .WithOne(ph => ph.Patient)
            .HasForeignKey<Patient>(p => p.PatientHistoryId)
            .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
    }
}
