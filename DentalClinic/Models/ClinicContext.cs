using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DentalClinic.Models
{
    public class ClinicContext : IdentityDbContext<ApplicationUser>
    {
        public ClinicContext(DbContextOptions<ClinicContext> options) : base(options) { }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Branch> Branchs { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Doctor_Work_Branch> DoctorWorkBranchs { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientHistory> PatientsHistory { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public virtual DbSet<outgoings> outgoings { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Call base for Identity configuration

            // One-to-one relationship between Branch and Secretary (ApplicationUser)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Secretary)
                .WithOne(u => u.Branch)
                .HasForeignKey<Branch>(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-one relationship between Doctor and Admin (ApplicationUser)
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Admin)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many relationship between Doctor and Branch through Doctor_Work_Branch
            modelBuilder.Entity<Doctor_Work_Branch>(entity =>
            {
                entity.HasKey(dwb => new { dwb.DoctorWorkBranchId, dwb.DoctorID, dwb.BranchID });

                entity.Property(dwb => dwb.IsWork)
                    .HasDefaultValue(true);

                entity.Property(dwb => dwb.StartTime)
                    .HasDefaultValue(new TimeSpan(13, 0, 0));

                entity.Property(dwb => dwb.EndTime)
                    .HasDefaultValue(new TimeSpan(1, 0, 0));

                entity.HasOne(dwb => dwb.Doctor)
                    .WithMany(d => d.DoctorWorkBranches)
                    .HasForeignKey(dwb => dwb.DoctorID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dwb => dwb.Branch)
                    .WithMany(b => b.DoctorWorkBranches)
                    .HasForeignKey(dwb => dwb.BranchID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration for PhoneNumber entity
            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Phonenumber)
                    .IsRequired()
                    .HasColumnType("nchar(11)");

                entity.HasIndex(p => p.Phonenumber)
                    .IsUnique();  // Ensure phone numbers are unique
            });

            // Configuration for PatientHistory and Patient
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.PatientHistory)
                .WithOne(ph => ph.Patient)
                .HasForeignKey<Patient>(p => p.PatientHistoryId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete on patient history

            // Adjust properties for PatientHistory fields
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
