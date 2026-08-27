namespace Exam.App.Services.Dtos.Patients
{
    public class UpdatePatientDto
    {
        public required string Name { get; set; }
        public int AnimalSpeciesId { get; set; }
        public DateOnly DateOfBirth { get; set; }

        // Id odabranog veterinara - veterinar SME da se menja.
        public required string VetId { get; set; }

        // NAMERNO nema OwnerUsername/OwnerId ovde - vlasnik se ne moze menjati nakon kreiranja (tacka 1 iz zadatka).
    }
}