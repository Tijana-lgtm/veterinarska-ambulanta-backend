namespace Exam.App.Services.Dtos.Patients
{
    public class CreatePatientDto
    {
        public required string Name { get; set; }
        public int AnimalSpeciesId { get; set; }
        public DateOnly DateOfBirth { get; set; }

        // Korisnicko ime vlasnika - sistem ga pronalazi i vezuje za ljubimca (tacka 1 iz zadatka).
        public required string OwnerUsername { get; set; }

        // Id odabranog veterinara (ApplicationUser.Id je string).
        public required string VetId { get; set; }
    }
}
