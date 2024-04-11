using Grpc.Core;
using GrpcService.Data;
using GrpcService.Protos;

namespace GrpcService.Services
{
    public class PatientAppointmentImpl :
        PatientAppointmentService.PatientAppointmentServiceBase
    {
        private readonly ILogger<PatientAppointmentImpl> _logger;
        private readonly ApplicationDbContext _context;

        public PatientAppointmentImpl(
            ApplicationDbContext context,
            ILogger<PatientAppointmentImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task GetPatientAppointmentSummary(GetPatientAppointmentSummaryRequest request,
            IServerStreamWriter<GetPatientAppointmentSummaryResponse> responseStream, ServerCallContext context)
        {
            var result = _context.Patients.GroupJoin(
               _context.Appointments,
               patient => patient.Id,
               appointment => appointment.PatientId,
               (patient, appointments) => new { patient, appointments }
            ).SelectMany(
                pa => pa.appointments.DefaultIfEmpty(),
                (pa, appointment) => new { pa.patient, appointment }
            ).GroupBy(
                pa => new { pa.patient.FirstName, pa.patient.LastName }
            ).Select(pa => new
            {
                PatientName = string.Concat(pa.Key.FirstName, " ", pa.Key.LastName),
                TotalAppointments = pa.Count(x => x.appointment != null),
                FirstAppointment = pa.Where(x => x.appointment != null).Min(x => x.appointment!.AppointmentDate),
                LastAppointment = pa.Where(x => x.appointment != null).Max(x => x.appointment!.AppointmentDate)
            });

            foreach (var item in result)
            {
                await responseStream.WriteAsync(new GetPatientAppointmentSummaryResponse
                {
                    PatientName = item.PatientName,
                    FirstAppointment = item.FirstAppointment.ToString(),
                    LastAppointment = item.LastAppointment.ToString(),
                    TotalAppointments = item.TotalAppointments
                });

                await Task.Delay(1000);
            }
        }
    }
}
