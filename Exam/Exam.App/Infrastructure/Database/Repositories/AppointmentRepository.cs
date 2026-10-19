using Exam.App.Domain;
using Exam.App.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exam.App.Infrastructure.Database.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task <List<Appointment>> GetUpcomingByVetAsync (string vetId)
        {
            var now = DateTime.UtcNow;
            return await _context.Appointments
                .Include(a=> a.Patient)
                    .ThenInclude(p=> p.AnimalSpecies)
                .Include(a=> a.Vet)
                .Where(a=>a.VetId == vetId && a.StartTime >= now)
                .OrderBy(a=>a.StartTime)
                .ToListAsync();
        }

        public async Task <List<Appointment>> GetByVetAsync (string vetId)
        {
            return await _context.Appointments
                .Include(a=> a.Patient)
                    .ThenInclude (p=> p.AnimalSpecies)
                .Include(a=> a.Vet)
                .Where(a=> a.VetId== vetId)
                .OrderBy(a=>a.StartTime)
                .ToListAsync ();
        }

        public async Task <Appointment?> GetByIdAsync (int id)
        {
            return await _context.Appointments
                .Include (a=> a.Patient)
                    .ThenInclude (p=> p.AnimalSpecies)
                .Include (a=> a.Vet)
                .FirstOrDefaultAsync (a=> a.Id == id);
        }

        public async Task <Appointment> AddAsync (Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task <Appointment> UpdateAsync (Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> ExistsOverlappingAsync(string vetId, DateTime startTime, int durationMinutes)
        {
            var endTime = startTime.AddMinutes(durationMinutes);

            // Dva termina se preklapaju ako pocetak jednog pada pre kraja drugog, u oba smera.
            // Otkazani pregledi se ne racunaju u proveru dostupnosti (3.2).
            return await _context.Appointments
                .Where(a=> a.VetId == vetId && !a.IsCancelled)
                .AnyAsync (a=> a.StartTime < endTime && startTime<a.StartTime.AddMinutes(durationMinutes));
        }
    }
}
