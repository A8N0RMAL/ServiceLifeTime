var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ServiceA>();
builder.Services.AddScoped<ServiceB>();

// Testing AddTransient LifeTime
builder.Services.AddTransient<RequestTracker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/check", (ServiceA serviceA, ServiceB serviceB) =>
{
    return Results.Ok(
        new
        {
            A = serviceA.GetInfo(),
            B = serviceB.GetInfo()
        }
    );
});

app.MapGet("/check2", (ServiceA serviceA) =>
{
    return Results.Ok(serviceA.GetInfo());
});

app.Run();

public class RequestTracker
{
    public string TrackerId = Guid.NewGuid().ToString()[..8];
}

public class ServiceA(RequestTracker tracker)
{
    public string GetInfo() => $"A -> {tracker.TrackerId}";
}

public class ServiceB(RequestTracker tracker)
{
    public string GetInfo() => $"B -> {tracker.TrackerId}";
}