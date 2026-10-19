using System.Security.Claims;
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

        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _appointmentsService.GetMyAppointmentsAsync(username!));
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

        [HttpPut("{id}/cancel")]
        public async Task <IActionResult> Cancel (int id, [FromBody] CancelAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cancelled = await _appointmentsService.CancelAsync(id, username!, dto);
            return Ok(cancelled);
        }

        [HttpPost("{id}/report")]
        public async Task<IActionResult> SubmitReport(int id, [FromBody] ReportDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _appointmentsService.SubmitReportAsync(id, username!, dto);
            return Ok(result);
        }

        [HttpPut("{id}/report")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _appointmentsService.UpdateReportAsync(id, username!, dto);
            return Ok(result);
        }
    }
}
