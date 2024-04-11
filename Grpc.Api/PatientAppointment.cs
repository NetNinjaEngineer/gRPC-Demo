namespace Grpc.Api
{
    public class PatientAppointment
    {
        public string? PatientName { get; set; }
        public string? FirstAppointment { get; set; }
        public string? LastAppointment { get; set; }
        public int TotalAppointments { get; set; }
    }
}
