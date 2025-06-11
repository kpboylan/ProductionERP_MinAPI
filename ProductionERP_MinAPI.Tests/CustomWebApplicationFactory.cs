using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using ProductionERP_MinAPI.Model;
using ProductionERP_MinAPI.Service;

namespace ProductionERP_MinAPI.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IAzureServiceBusSvc<Material>> BusServiceMock { get; private set; } = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(GetApiProjectPath());

            builder.ConfigureServices(services =>
            {
                // Remove the real implementation
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IAzureServiceBusSvc<Material>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Register the mock instead
                BusServiceMock = new Mock<IAzureServiceBusSvc<Material>>();
                services.AddScoped(_ => BusServiceMock.Object);
            });

            return base.CreateHost(builder);
        }

        private static string GetApiProjectPath()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var relativePath = Path.Combine(projectDir, @"..\..\..\..\ProductionERP_MinAPI");
            var fullPath = Path.GetFullPath(relativePath);

            if (!File.Exists(Path.Combine(fullPath, "ProductionERP_MinAPI.csproj")))
            {
                throw new InvalidOperationException($"Could not find API project at: {fullPath}");
            }

            return fullPath;
        }
    }
}
