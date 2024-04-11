using GrpcService.Entities.Common;

namespace GrpcService.Entities
{
    public class Patient : Person
    {
        public string? Email { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
