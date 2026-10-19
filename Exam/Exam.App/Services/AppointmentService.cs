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

        public async Task <List<ShowAppointmentDto>> GetMyAppointmentsAsync(string username)
        {
            var vet = await _userManager.FindByNameAsync(username);
            if (vet == null)
                throw new BadRequestException("Korisnik ne postoji.");

            var appointments = await _appointmentRepository.GetByVetAsync(vet.Id);
            var result = new List <ShowAppointmentDto>();

            foreach (var appointment in appointments)
            {
                var dto = _mapper.Map<ShowAppointmentDto>(appointment);
                dto.CanEditReport = CanEditReport(appointment);
                result.Add(dto);
            }
            return result;
        }

        public async Task<ShowAppointmentDto> CancelAsync(int id, string username, CancelAppointmentDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new NotFoundException(id);

            var currentUser = await _userManager.FindByNameAsync(username);
            if (currentUser == null || appointment.VetId != currentUser.Id)
                throw new BadRequestException("Mozete otkazati samo svoje zakazane preglede.");

            if (appointment.IsCancelled)
                throw new BadRequestException("Pregled je vec otkazan.");

            appointment.IsCancelled = true;
            appointment.CancellationReason = dto.Reason;

            await _appointmentRepository.UpdateAsync(appointment);

            var fullyLoaded = await _appointmentRepository.GetByIdAsync(id);
            return _mapper.Map<ShowAppointmentDto>(fullyLoaded!);
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

        // Podnosenje izvestaja (3.3) - samo dodeljeni veterinar, samo jednom, ne za otkazan pregled.
        public async Task<ShowAppointmentDto> SubmitReportAsync(int id, string username, ReportDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new NotFoundException(id);

            var currentUser = await _userManager.FindByNameAsync(username);
            if (currentUser == null || appointment.VetId != currentUser.Id)
                throw new BadRequestException("Mozete podneti izvestaj samo za svoje zakazane preglede.");

            if (appointment.IsCancelled)
                throw new BadRequestException("Ne mozete podneti izvestaj za otkazan pregled.");

            if (appointment.ReportCreatedAt != null)
                throw new BadRequestException("Izvestaj je vec podnet za ovaj pregled.");

            appointment.Weight = dto.Weight;
            appointment.Anamnesis = dto.Anamnesis;
            appointment.ReportCreatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            var fullyLoaded = await _appointmentRepository.GetByIdAsync(id);
            var resultDto = _mapper.Map<ShowAppointmentDto>(fullyLoaded!);
            resultDto.CanEditReport = CanEditReport(fullyLoaded!);
            return resultDto;
        }

        // Izmena vec podnetog izvestaja (3.3) - dozvoljeno samo unutar roka od 3 radna dana.
        public async Task<ShowAppointmentDto> UpdateReportAsync(int id, string username, ReportDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new NotFoundException(id);

            var currentUser = await _userManager.FindByNameAsync(username);
            if (currentUser == null || appointment.VetId != currentUser.Id)
                throw new BadRequestException("Mozete izmeniti izvestaj samo za svoje zakazane preglede.");

            if (appointment.ReportCreatedAt == null)
                throw new BadRequestException("Izvestaj jos nije podnet za ovaj pregled.");

            if (!CanEditReport(appointment))
                throw new BadRequestException("Rok za izmenu izvestaja je istekao.");

            appointment.Weight = dto.Weight;
            appointment.Anamnesis = dto.Anamnesis;
            // ReportCreatedAt se NE menja - rok se i dalje racuna od originalnog podnosenja.

            await _appointmentRepository.UpdateAsync(appointment);

            var fullyLoaded = await _appointmentRepository.GetByIdAsync(id);
            var resultDto = _mapper.Map<ShowAppointmentDto>(fullyLoaded!);
            resultDto.CanEditReport = CanEditReport(fullyLoaded!);
            return resultDto;
        }
        private static bool CanEditReport (Appointment appointment)
        {
            if (appointment.ReportCreatedAt == null)
                return false;

            var reportDate = DateOnly.FromDateTime(appointment.ReportCreatedAt.Value);
            var deadline = AddBusinessDays(reportDate, 2);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return today <= deadline;
        }

        private static DateOnly AddBusinessDays (DateOnly startDate, int businessDaysToAdd)
        {
            var date = startDate;
            var addedDays = 0;

            while (addedDays< businessDaysToAdd)
            {
                date = date.AddDays(1);

                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    addedDays = addedDays + 1;
                }
            }
            return date;
        }
    }
}
