using Exam.App.Services;
using Exam.App.Services.Dtos.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exam.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Veterinar,Pomocnik")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentsService;

        public AppointmentsController(IAppointmentService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet]
        public async Task <IActionResult> GetUpcomingByVet([FromQuery] string vetId)
        {
            return Ok (await _appointmentsService.GetUpcomingByVetAsync(vetId));
        }

        [HttpPost]
        public async Task <IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _appointmentsService.CreateAsync(dto);
            return Ok(created);
        }
    }
}
