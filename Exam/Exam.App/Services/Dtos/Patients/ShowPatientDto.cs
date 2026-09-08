namespace Exam.App.Services.Dtos.Patients
{
    public class ShowPatientDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public int AnimalSpeciesId { get; set; }
        public required string AnimalSpeciesName { get; set; }
        
        public required string OwnerFullName { get; set; }
        
        public required string VetId { get; set; }
        public required string VetFullName { get; set; }

    }
}
