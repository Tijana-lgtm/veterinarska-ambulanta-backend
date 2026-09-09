using Exam.App.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exam.App.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet <AnimalSpecies> AnimalSpecies { get; set; }
    public DbSet <Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed Roles
        modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "d290f1ee-6c54-4b01-90e6-d701748f0851", Name = "Veterinar", NormalizedName = "VETERINAR" },
                new IdentityRole { Id = "4a9e5f2b-8c1d-4e6a-9b3f-2d7c8a1e6f40", Name = "Pomocnik", NormalizedName = "POMOCNIK" },
                new IdentityRole { Id = "7b3d6e91-1a4f-4c8b-8e2a-5f9d3c7b2a11", Name = "VlasnikLjubimca", NormalizedName = "VLASNIKLJUBIMCA" }
        );

        // Seed Entities

        modelBuilder.Entity<AnimalSpecies>(e =>
        {
            e.HasData(
                new AnimalSpecies { Id = 1, Name = "Pas" },
                new AnimalSpecies { Id = 2, Name = "Mačka" },
                new AnimalSpecies { Id = 3, Name = "Papagaj" },
                new AnimalSpecies { Id = 4, Name = "Kornjača" },
                new AnimalSpecies { Id = 5, Name = "Zec" },
                new AnimalSpecies { Id = 6, Name = "Hrčak" }
            );
        });

        //Pacijent - vrsta zivotinje
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.AnimalSpecies)
            .WithMany()
            .HasForeignKey (p => p.AnimalSpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        //Pacijent - vlasnik (ApplicationUser)
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        //Pacijent - veterinar (ApplicationUser)
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Vet)
            .WithMany()
            .HasForeignKey(p => p.VetId)
            .OnDelete(DeleteBehavior.Restrict);

        //Pregled - pacijent
        modelBuilder.Entity<Appointment>()
            .HasOne(a=> a.Patient)
            .WithMany()
            .HasForeignKey(a=>a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        //Pregled - veterinar (ApplicationUser)
        modelBuilder.Entity<Appointment>()
            .HasOne(a=> a.Vet)
            .WithMany()
            .HasForeignKey(a=> a.VetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
