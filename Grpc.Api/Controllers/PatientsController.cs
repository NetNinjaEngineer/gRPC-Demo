using Grpc.Api.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientAppointmentService.PatientAppointmentServiceClient _client;

        public PatientsController(PatientAppointmentService.PatientAppointmentServiceClient client)
        {
            _client = client;
        }

        [HttpGet("PatientAppointmentSummaries")]
        public async Task<IActionResult> GetPatientAppointmentSummaries()
        {
            var summaries = new List<PatientAppointment>();

            try
            {
                using var summary = _client.GetPatientAppointmentSummary(
                    new GetPatientAppointmentSummaryRequest());

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
            catch (RpcException ex) when (ex.StatusCode == Core.StatusCode.Cancelled)
            {
                return StatusCode((int)Core.StatusCode.Cancelled, "Stream cancelled.");
            }
            catch (RpcException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }

        }
    }
}
