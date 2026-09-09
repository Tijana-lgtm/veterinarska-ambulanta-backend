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


    }
}
