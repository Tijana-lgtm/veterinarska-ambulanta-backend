namespace Exam.App.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;


        // Veterinar u trenutku zakazivanja - ne uzima se transitivno preko Patient.VetId,
        // jer se veterinar dodeljen pacijentu moze naknadno promeniti (UpdatePatientDto).
        public required string VetId { get; set; }
        public ApplicationUser Vet { get; set; } = null!;

        // Otkazivanje (3.2) - pregled se ne brise, ostaje vidljiv ali zasivljen,
        // i ne racuna se u proveru dostupnosti veterinara (2.3).
        public bool IsCancelled { get; set; }
        public string? CancellationReason { get; set; }
    
        public double? Weight { get; set; }
        public string? Anamnesis { get; set; }
        public DateTime? ReportCreatedAt { get; set; }
    }
}
