using AutoMapper;
using Exam.App.Domain;
using Exam.App.Domain.Repositories;
using Exam.App.Services.Dtos.Appointments;
using Exam.App.Services.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Exam.App.Services
{
    public class AppointmentService : IAppointmentService
    {
        private const int AppointmentDurationMinutes = 20;

        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AppointmentService (IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, UserManager<ApplicationUser> userManager, IMapper mapper)    
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task <List<ShowAppointmentDto>> GetUpcomingByVetAsync(string vetId)
        {
            var appointments = await _appointmentRepository.GetUpcomingByVetAsync(vetId);
            return appointments.Select (a=> _mapper.Map<ShowAppointmentDto>(a)).ToList();
        }

        public async Task <ShowAppointmentDto> CreateAsync (CreateAppointmentDto dto)
        {
            var startTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);

            var vet = await _userManager.FindByIdAsync(dto.VetId);
            if (vet == null || !await _userManager.IsInRoleAsync(vet, "Veterinar"))
                throw new BadRequestException("Odabrani veterinar ne postoji.");

            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new NotFoundException(dto.PatientId);

            if (patient.VetId != dto.VetId)
                throw new BadRequestException("Odabrani pacijent nema dodeljenog ovog veterinara.");
        
            var isOverlapping = await _appointmentRepository.ExistsOverlappingAsync(dto.VetId, startTime, AppointmentDurationMinutes);
            if (isOverlapping)
                throw new BadRequestException("Veterinar nije dostupan u izabranom terminu.");

            var appointment = new Appointment
            {
                StartTime = startTime,
                PatientId = dto.PatientId,
                VetId = dto.VetId,
            };

            var created = await _appointmentRepository.AddAsync(appointment);

            var fullyLoaded = await _appointmentRepository.GetByIdAsync(created.Id);
            return _mapper.Map<ShowAppointmentDto>(fullyLoaded!);
        }
    }
}
