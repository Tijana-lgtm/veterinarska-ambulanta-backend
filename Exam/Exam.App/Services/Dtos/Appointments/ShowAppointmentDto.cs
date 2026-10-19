namespace Exam.App.Services.Dtos.Appointments
{
    public class ShowAppointmentDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }

        public int PatientId { get; set; }
        public required string PatientName { get; set; }
        public required string AnimalSpeciesName { get; set; }
        public int PatientAge { get; set; }

        public required string VetId { get; set; }
        public required string VetFullName { get; set; }

        public bool IsCancelled { get; set; }
        public string? CancellationReason { get; set; }

        public double? Weight { get; set; }
        public string? Anamnesis { get; set; }
        public bool HasReport { get; set; }
        public bool CanEditReport { get; set; }
    }
}
