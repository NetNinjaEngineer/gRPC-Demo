using GrpcService.Data;
using GrpcService.Services;
using Microsoft.EntityFrameworkCore;

namespace GrpcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HealthCareDB")));

            var app = builder.Build();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>()
                .EnableGrpcWeb();

            app.MapGrpcService<PatientAppointmentImpl>()
                .EnableGrpcWeb();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}