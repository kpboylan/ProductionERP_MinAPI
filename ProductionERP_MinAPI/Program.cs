using ProductionERP_MinAPI.Model;
using ProductionERP_MinAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // *** IMPORTANT: Replace with your Angular app's URL ***
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // If your Angular app sends credentials (like cookies or auth headers for MSAL)
        });

    // You can also add a more permissive policy for development
    options.AddPolicy("AllowAllDevelopment",
        builder =>
        {
            builder.AllowAnyOrigin() // NOT recommended for production
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MessageBus>();
builder.Services.AddScoped<IAzureServiceBusSvc<Material>, AzureServiceBusSvc<Material>>();
builder.Services.AddScoped<IAzureServiceBusSvc<Product>, AzureServiceBusSvc<Product>>();
builder.Services.AddTransient<IEmailServiceBusPublisher, EmailServiceBusPublisher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowSpecificOrigin");


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

app.MapPost("/product", async (Product product, IAzureServiceBusSvc<Product> busService) =>
{
    try
    {
        var result = await busService.PublishAsync(product);
        return Results.Created($"/product/{product.ProductId}", result);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to publish message: {ex.Message}");
    }
});

app.Run();

public partial class Program { }
