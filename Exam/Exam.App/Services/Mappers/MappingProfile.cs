using AutoMapper;
using Exam.App.Domain;
using Exam.App.Services.Dtos;
using Exam.App.Services.Dtos.Patients;
using Exam.App.Services.Dtos.Appointments;

namespace Exam.App.Services.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ProfileDto>();

            // CreatePatientDto -> Patient: OwnerId/VetId se NE mapiraju automatski,
            // Service ih postavlja rucno nakon sto pronadje vlasnika/veterinara preko UserManager-a.
            CreateMap<CreatePatientDto, Patient>()
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.VetId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Vet, opt => opt.Ignore())
                .ForMember(dest => dest.AnimalSpecies, opt => opt.Ignore());

            // UpdatePatientDto -> Patient: Id i OwnerId se namerno ne diraju (vlasnik je nemenjiv).
            CreateMap<UpdatePatientDto, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.VetId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Vet, opt => opt.Ignore())
                .ForMember(dest => dest.AnimalSpecies, opt => opt.Ignore());

            // Patient -> ShowPatientDto: spajanje imena i prezimena za vlasnika i veterinara.
            CreateMap<Patient, ShowPatientDto>()
                .ForMember(dest => dest.AnimalSpeciesName, opt => opt.MapFrom(src => src.AnimalSpecies.Name))
                .ForMember(dest => dest.OwnerFullName, opt => opt.MapFrom(src => src.Owner.Name + " " + src.Owner.Surname))
                .ForMember(dest => dest.VetFullName, opt => opt.MapFrom(src => src.Vet.Name + " " + src.Vet.Surname));

            // ApplicationUser -> VetDto: za dropdown listu veterinara (GET /api/Patients/vets).
            CreateMap<ApplicationUser, VetDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name + " " + src.Surname));

            // AnimalSpecies -> AnimalSpeciesDto: za dropdown listu vrsta (GET /api/Patients/species).
            CreateMap<AnimalSpecies, AnimalSpeciesDto>();

            // Appointment -> ShowAppointmentDto: podaci o pacijentu i veterinaru za listu pregleda.
            CreateMap<Appointment, ShowAppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.AnimalSpeciesName, opt => opt.MapFrom(src => src.Patient.AnimalSpecies.Name))
                .ForMember(dest => dest.PatientAge, opt => opt.MapFrom(src => CalculateAge(src.Patient.DateOfBirth)))
                .ForMember(dest => dest.VetFullName, opt => opt.MapFrom(src => src.Vet.Name + " " + src.Vet.Surname));
        }

        // Racuna starost (u godinama) na osnovu datuma rodjenja - koristi se za ShowAppointmentDto.PatientAge.
        private static int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
                age--;

            return age;
        }
    }
}