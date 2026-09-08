using AutoMapper;
using Exam.App.Domain;
using Exam.App.Domain.Repositories;
using Exam.App.Services.Dtos.Patients;
using Exam.App.Services.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Exam.App.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly UserManager <ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public PatientService (IPatientRepository patientRepository, UserManager<ApplicationUser> userManager, IMapper mapper) 
        {
            _patientRepository = patientRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task <List<ShowPatientDto>> GetAllAsync(string? vetId, string? name, int? animalSpeciesId, int? ageFrom, int? ageTo) 
        {
            var patients = await _patientRepository.GetAllAsync(vetId, name, animalSpeciesId, ageFrom, ageTo);
            return patients.Select(p=> _mapper.Map<ShowPatientDto>(p)).ToList();
        }

        public async Task<ShowPatientDto> GetByIdAsync (int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new NotFoundException(id);

            return _mapper.Map<ShowPatientDto>(patient);
        }

        public async Task <ShowPatientDto> CreateAsync (CreatePatientDto dto)
        {
            var owner = await _userManager.FindByNameAsync (dto.OwnerUsername);
            if (owner == null)
                throw new BadRequestException($"Vlasnik sa korisnickim imenom '{dto.OwnerUsername}' ne postoji.");

            var vet = await _userManager.FindByIdAsync(dto.VetId);
            if (vet == null)
                throw new BadRequestException("Odabrani veterinar ne postoji.");

            var patient = _mapper.Map<Patient>(dto);
            patient.OwnerId = owner.Id;
            patient.VetId = vet.Id;

            var created = await _patientRepository.AddAsync(patient);

            // Ponovo ucitaj sa Include-ovima (AnimalSpecies/Owner/Vet) da ShowPatientDto ima puna imena.
            var fullyLoaded = await _patientRepository.GetByIdAsync(created.Id);
            return _mapper.Map<ShowPatientDto>(fullyLoaded);
        }

        public async Task <ShowPatientDto> UpdateAsync (int id, UpdatePatientDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient==null)
                throw new NotFoundException(id);

            var vet = await _userManager.FindByIdAsync(dto.VetId);
            if (vet == null) 
                throw new BadRequestException("Odabrani veterinar ne postoji.");

            // OwnerId ostaje netaknut - UpdatePatientDto ga uopste ne sadrzi.
            _mapper.Map(dto, patient);
            patient.VetId = vet.Id;

            await _patientRepository.UpdateAsync(patient);

            var fullyLoaded = await _patientRepository.GetByIdAsync(id);
            return _mapper.Map<ShowPatientDto>(fullyLoaded);
        }

        public async Task DeleteAsync (int id)
        {
            var deleted = await _patientRepository.DeleteAsync(id);
            if(!deleted)
                throw new NotFoundException(id);
        }

        public async Task <List<VetDto>> GetVetsAsync ()
        {
            var vets = await _userManager.GetUsersInRoleAsync("Veterinar");
            return vets
                .Select (v=> _mapper.Map<VetDto>(v))
                .ToList();
        }

        public async Task <List<AnimalSpeciesDto>> GetSpeciesAsync()
        {
            var species = await _patientRepository.GetAllSpeciesAsync();
            return species.Select(s => _mapper.Map<AnimalSpeciesDto>(s)).ToList(); ;
        }

    }
}
