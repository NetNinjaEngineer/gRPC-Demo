using Grpc.Api.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PatientAppointmentService.PatientAppointmentServiceClient _client;

        public PatientsController(IConfiguration configuration)
        {
            _configuration = configuration;
            var channel = GrpcChannel.ForAddress(_configuration!.GetSection("GrpcUrl").Value!);
            _client = new PatientAppointmentService.PatientAppointmentServiceClient(channel);
        }

        [HttpGet("PatientAppointmentSummaries")]
        public async Task<IActionResult> GetPatientAppointmentSummaries()
        {
            using var summary = _client.GetPatientAppointmentSummary(new GetPatientAppointmentSummaryRequest());

            var summaries = new List<PatientAppointment>();

            var result = summary.ResponseStream.ReadAllAsync();

            await foreach (var item in result)
                summaries.Add(new PatientAppointment
                {
                    PatientName = item.PatientName,
                    FirstAppointment = item.FirstAppointment,
                    LastAppointment = item.LastAppointment,
                    TotalAppointments = item.TotalAppointments
                });

            return Ok(summaries);
        }
    }
}
