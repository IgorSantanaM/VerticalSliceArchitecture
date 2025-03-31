using TravelInspiration.API;
using TravelInspiration.API.Shared.Slices;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddProblemDetails();

services.AddHttpClient();

services.RegisterApplicationServices();
services.RegisterPersistenceServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.MapSliceEndpoints();

app.Run();