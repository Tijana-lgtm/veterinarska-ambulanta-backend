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
    }
}
