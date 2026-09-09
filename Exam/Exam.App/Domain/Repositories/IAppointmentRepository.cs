namespace Exam.App.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetUpcomingByVetAsync(string vetId);
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> AddAsync(Appointment appointment);
        Task<bool> ExistsOverlappingAsync(string vetId, DateTime startTime, int durationMinutes);
    }
}