using Exam.App.Services;
using Exam.App.Services.Dtos.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exam.App.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Veterinar,Pomocnik")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task <IActionResult> GetAll()
        {
            return Ok (await _patientService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetById (int id)
        {
            return Ok (await _patientService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task <IActionResult> Create ([FromBody] CreatePatientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _patientService.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _patientService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete (int id)
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
