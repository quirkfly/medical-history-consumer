using Microsoft.EntityFrameworkCore;
using PatientMedicalHistory.Models;

namespace PatientMedicalHistory.Data
{
    public class MedicalHistoryContext : DbContext
    {
        public MedicalHistoryContext(DbContextOptions<MedicalHistoryContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistoryEntry> MedicalHistoryEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.MedicalHistory)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId);
        }
    }
}