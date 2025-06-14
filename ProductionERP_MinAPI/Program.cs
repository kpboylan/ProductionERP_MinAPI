using ProductionERP_MinAPI.Model;
using ProductionERP_MinAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MessageBus>();
builder.Services.AddScoped<IAzureServiceBusSvc<Material>, AzureServiceBusSvc<Material>>();

builder.Services.AddScoped<IAzureServiceBusSvc<MaterialType>, AzureServiceBusSvc<MaterialType>>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


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

app.MapPost("/materialType", async (MaterialType materialType, IAzureServiceBusSvc<MaterialType> busService) =>
{
    try
    {
        var result = await busService.PublishAsync(materialType);
        return Results.Created($"/materialType/{materialType.MaterialTypeID}", result);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to publish message: {ex.Message}");
    }
});

app.Run();

public partial class Program { }
