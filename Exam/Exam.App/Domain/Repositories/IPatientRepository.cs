namespace Exam.App.Domain.Repositories
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync(string? vetId, string? name, int? animalSpeciesId, int? ageFrom, int? ageTo);
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);

        Task<List<AnimalSpecies>> GetAllSpeciesAsync();

    }
}
