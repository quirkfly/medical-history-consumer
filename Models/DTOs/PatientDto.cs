using System;
using System.Collections.Generic;

namespace PatientMedicalHistory.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public List<MedicalHistoryEntryDto> MedicalHistory { get; set; } = new List<MedicalHistoryEntryDto>();
    }

    public class MedicalHistoryEntryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; }   = string.Empty;
    }
}
