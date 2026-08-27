using Exam.App.Domain;
using Microsoft.AspNetCore.Identity;

namespace Exam.App.Infrastructure.Database;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        // Veterinar 1 
        var vet1 = await userManager.FindByNameAsync("marko.vet");
        if (vet1 == null)
        {
            vet1 = new ApplicationUser
            {
                UserName = "marko.vet",
                Email = "marko.jovanovic@vet.rs",
                Name = "Marko",
                Surname = "Jovanović",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(vet1, "Marko123!");
            await userManager.AddToRoleAsync(vet1, "Veterinar");
        }

        // Veterinar 2
        var vet2 = await userManager.FindByNameAsync("ana.vet");
        if (vet2 == null)
        {
            vet2 = new ApplicationUser
            {
                UserName = "ana.vet",
                Email = "ana.ilic@vet.rs",
                Name = "Ana",
                Surname = "Ilić",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(vet2, "Ana12345!");
            await userManager.AddToRoleAsync(vet2, "Veterinar");
        }

        // Pomoćnik
        var assistant1 = await userManager.FindByNameAsync("petar.pomocnik");
        if (assistant1 == null)
        {
            assistant1 = new ApplicationUser
            {
                UserName = "petar.pomocnik",
                Email = "petar.petrovic@vet.rs",
                Name = "Petar",
                Surname = "Petrović",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(assistant1, "Petar123!");
            await userManager.AddToRoleAsync(assistant1, "Pomocnik");
        }

        // Vlasnik ljubimca 1
        var owner1 = await userManager.FindByNameAsync("nikola.vlasnik");
        if (owner1 == null)
        {
            owner1 = new ApplicationUser
            {
                UserName = "nikola.vlasnik",
                Email = "nikola.nikolic@example.com",
                Name = "Nikola",
                Surname = "Nikolić",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(owner1, "Nikola123!");
            await userManager.AddToRoleAsync(owner1, "VlasnikLjubimca");
        }

        // Vlasnik ljubimca 2
        var owner2 = await userManager.FindByNameAsync("jovana.vlasnik");
        if (owner2 == null)
        {
            owner2 = new ApplicationUser
            {
                UserName = "jovana.vlasnik",
                Email = "jovana.jovanovic@example.com",
                Name = "Jovana",
                Surname = "Jovanović",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(owner2, "Jovana123!");
            await userManager.AddToRoleAsync(owner2, "VlasnikLjubimca");
        }

        // Nekoliko pacijenata radi lakseg testiranja CRUD-a u narednim danima.
        if (!dbContext.Patients.Any())
        {
            dbContext.Patients.AddRange(
                new Patient
                {
                    Name = "Reks",
                    DateOfBirth = new DateOnly(2015, 3, 12), // ~11 godina, vrsta Pas
                    AnimalSpeciesId = 1,
                    OwnerId = owner1.Id,
                    VetId = vet1.Id
                },
                new Patient
                {
                    Name = "Mica",
                    DateOfBirth = new DateOnly(2020, 7, 1), // vrsta Macka
                    AnimalSpeciesId = 2,
                    OwnerId = owner2.Id,
                    VetId = vet2.Id
                },
                new Patient
                {
                    Name = "Kesa",
                    DateOfBirth = new DateOnly(2010, 1, 20), // ~16 godina, vrsta Papagaj
                    AnimalSpeciesId = 3,
                    OwnerId = owner1.Id,
                    VetId = vet1.Id
                }
            );

            await dbContext.SaveChangesAsync();
        }
    }
}