using Exam.App.Domain;
using Exam.App.Domain.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Exam.App.Infrastructure.Database.Repositories
{
    public class PatientRepository: IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllAsync(string? vetId, string? name, int? animalSpeciesId, int? ageFrom, int? ageTo)
        {
            var query = _context.Patients
                .Include(p => p.AnimalSpecies)
                .Include(p => p.Owner)
                .Include(p => p.Vet)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(vetId)) 
            { 
                query = query.Where(p=>p.VetId== vetId);    
            }

            if (!string.IsNullOrWhiteSpace(name)) 
            { 
                query = query.Where (p => p.Name.ToLower().Contains(name.ToLower()));
            }

            if (animalSpeciesId.HasValue)
            {
                query = query.Where(p => p.AnimalSpeciesId == animalSpeciesId.Value);
            }
            // Starost se racuna iz DateOfBirth - "od X godina" znaci rodjen NAJKASNIJE tog datuma,
            // "do Y godina" znaci rodjen NAJRANIJE tog datuma.

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (ageFrom.HasValue)
            {
                var maxDateOfBirth = today.AddYears(-ageFrom.Value);
                query = query.Where(p=>p.DateOfBirth <= maxDateOfBirth);
            }

            if (ageTo.HasValue)
            {
                var minDateOfBirth = today.AddYears(-(ageTo.Value + 1)).AddDays(1);
                query = query.Where(p => p.DateOfBirth >= minDateOfBirth);
            }
            return await query.ToListAsync();

        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include (p => p.AnimalSpecies)
                .Include(p => p.Owner)
                .Include (p => p.Vet)
                .FirstOrDefaultAsync(p=> p.Id == id);
        }

        public async Task<Patient> AddAsync (Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdateAsync (Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task <bool> DeleteAsync (int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) 
                return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AnimalSpecies>> GetAllSpeciesAsync()
        {
            return await _context.AnimalSpecies.ToListAsync();
        }
    }
}
