using Grpc.Core;
using Grpc.Net.Client;
using GrpcService.Protos;

namespace GrpcClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7181");
            var client = new PatientAppointmentService.PatientAppointmentServiceClient(channel);
            var result = client.GetPatientAppointmentSummary(new GetPatientAppointmentSummaryRequest());
            var data = result.ResponseStream.ReadAllAsync();
            await foreach (var item in data)
            {
                Console.WriteLine(item);
            }
        }
    }
}
