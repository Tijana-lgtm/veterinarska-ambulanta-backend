using Exam.App.Services.Dtos.Patients;

namespace Exam.App.Services
{
    public interface IPatientService
    {
        Task<List<ShowPatientDto>> GetAllAsync(string? vetId, string? name, int? animalSpeciesId, int? ageFrom, int? ageTo);
        Task<ShowPatientDto> GetByIdAsync(int id);
        Task<ShowPatientDto> CreateAsync(CreatePatientDto dto);
        Task<ShowPatientDto> UpdateAsync(int id, UpdatePatientDto dto);
        Task DeleteAsync(int id);
        Task<List<VetDto>> GetVetsAsync();
        Task<List<AnimalSpeciesDto>> GetSpeciesAsync();
    }
}