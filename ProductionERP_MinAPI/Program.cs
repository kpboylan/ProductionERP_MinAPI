using ProductionERP_MinAPI.Model;
using ProductionERP_MinAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MessageBus and AzureServiceBusSvc<Material> as services
builder.Services.AddSingleton<MessageBus>();
builder.Services.AddScoped<IAzureServiceBusSvc<Material>, AzureServiceBusSvc<Material>>();

var app = builder.Build();

// 📜 Enable Swagger only in development
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(); // Opens Swagger UI in browser
//}


app.MapPost("/material", async (Material material, IAzureServiceBusSvc<Material> busService) =>
{
    try
    {
        var result = await busService.PublishAsync(material);
        return Results.Created($"/material/{material.MaterialID}", result);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to publish message: {ex.Message}");
    }
});

app.Run();
