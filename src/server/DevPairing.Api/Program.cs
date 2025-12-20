using DevPairing.Api.Data;
using DevPairing.Api.Endpoints;
using DevPairing.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<NtfyOptions>(builder.Configuration.GetSection("Ntfy"));
builder.Services.AddHttpClient<INtfyService, NtfyService>();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.AddNpgsqlDbContext<AppDbContext>("devpairingdb");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    
    // Seed data
    await SeedData.InitializeAsync(app.Services);
}

app.UseCors();

app.UseHttpsRedirection();



app.UseDefaultFiles();

app.UseStaticFiles();



app.MapDevGroupEndpoints();

app.MapUserEndpoints();

app.MapMembershipEndpoints();

app.MapSlotEndpoints();

app.MapSignupEndpoints();

app.MapPreferencesEndpoints();

app.MapAdminEndpoints();



app.MapFallbackToFile("index.html");





app.Run();
