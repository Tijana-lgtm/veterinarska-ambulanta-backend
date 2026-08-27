using Exam.App.Services.Dtos.Patients;

namespace Exam.App.Services
{
    public interface IPatientService
    {
        Task<List<ShowPatientDto>> GetAllAsync();
        Task<ShowPatientDto> GetByIdAsync(int id);
        Task<ShowPatientDto> CreateAsync(CreatePatientDto dto);
        Task<ShowPatientDto> UpdateAsync(int id, UpdatePatientDto dto);
        Task DeleteAsync(int id);
    }
}