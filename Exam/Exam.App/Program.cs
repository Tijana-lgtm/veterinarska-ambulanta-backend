using Exam.App;
using Exam.App.Controllers.Middleware;
using Exam.App.Infrastructure.Database;
using Exam.App.Services;
using Exam.App.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using Exam.App.Domain.Repositories;
using Exam.App.Infrastructure.Database.Repositories;

var builder = WebApplication.CreateBuilder(args);

Startup.AddLogging(builder);
Startup.AddSwagger(builder);
Startup.AddCors(builder);
Startup.AddAuthenticationAndAuthorization(builder);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Service and Repository Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();