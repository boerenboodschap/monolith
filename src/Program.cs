using bb_api.Models;
using bb_api.Services;

using Prometheus;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

// Database
builder.Services.Configure<ProductDatabaseSettings>(
    builder.Configuration.GetSection("ProductDatabase"));
builder.Services.Configure<FarmDatabaseSettings>(
    builder.Configuration.GetSection("FarmDatabase"));

builder.Services.AddSingleton<ProductsService>();
builder.Services.AddSingleton<FarmsService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();

// Capture metrics about all received HTTP requests.
app.UseHttpMetrics();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    // Enable the /metrics page to export Prometheus metrics.
    // Open http://localhost:5099/metrics to see the metrics.
    //
    // Metrics published in this sample:
    // * built-in process metrics giving basic information about the .NET runtime (enabled by default)
    // * metrics from .NET Event Counters (enabled by default, updated every 10 seconds)
    // * metrics from .NET Meters (enabled by default)
    // * metrics about requests made by registered HTTP clients used in SampleService (configured above)
    // * metrics about requests handled by the web app (configured above)
    // * ASP.NET health check statuses (configured above)
    // * custom business logic metrics published by the SampleService class
    endpoints.MapMetrics();

    // Auth0
    endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.Run();
