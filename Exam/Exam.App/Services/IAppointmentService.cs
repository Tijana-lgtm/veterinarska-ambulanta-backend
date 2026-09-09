using Exam.App.Services.Dtos.Appointments;

namespace Exam.App.Services
{
    public interface IAppointmentService
    {
        Task<List<ShowAppointmentDto>> GetUpcomingByVetAsync(string vetId);
        Task<ShowAppointmentDto> CreateAsync(CreateAppointmentDto dto);
    }
}
