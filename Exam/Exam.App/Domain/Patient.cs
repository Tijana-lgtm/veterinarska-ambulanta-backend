namespace Exam.App.Domain
{
    public class Patient
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int AnimalSpeciesId { get; set; }
        public AnimalSpecies AnimalSpecies { get; set; } = null!;
        
        // Vlasnik ljubimca - postavlja se pri kreiranju i nije moguce menjati kasnije (Servis nivo, Dan 2).
        public required string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        // Veterinar dodeljen pacijentu - moze se menjati.
        public required string VetId { get; set; }
        public ApplicationUser Vet { get; set; } = null!;
    }
}
