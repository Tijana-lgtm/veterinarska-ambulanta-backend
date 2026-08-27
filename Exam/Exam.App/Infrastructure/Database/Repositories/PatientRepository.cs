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

        public async Task<List<Patient>> GetAllAsync ()
        {
            return await _context.Patients
                .Include(p => p.AnimalSpecies)
                .Include(p => p.Owner)
                .Include(p => p.Vet)
                .ToListAsync();
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
    }
}
