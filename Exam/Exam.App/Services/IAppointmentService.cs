using Exam.App.Services.Dtos.Appointments;

namespace Exam.App.Services
{
    public interface IAppointmentService
    {
        Task<List<ShowAppointmentDto>> GetUpcomingByVetAsync(string vetId);
        Task<List<ShowAppointmentDto>> GetMyAppointmentsAsync(string username);
        Task<ShowAppointmentDto> CreateAsync(CreateAppointmentDto dto);
        Task<ShowAppointmentDto> CancelAsync(int id, string username, CancelAppointmentDto dto);
        Task<ShowAppointmentDto> SubmitReportAsync(int id, string username, ReportDto dto);
        Task<ShowAppointmentDto> UpdateReportAsync (int id, string username, ReportDto dto);
    }
}
