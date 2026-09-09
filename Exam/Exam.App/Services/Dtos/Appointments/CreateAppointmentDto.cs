namespace Exam.App.Services.Dtos.Appointments
{
    public class CreateAppointmentDto
    {
        public DateTime StartTime { get; set; }
        public int PatientId { get; set; }

        // Veterinar za kog se pregled zakazuje - front ga salje eksplicitno
        // (odabran je pre nego sto je spisak pacijenata uopste ucitan).
        public required string VetId { get; set; }
    }
}
