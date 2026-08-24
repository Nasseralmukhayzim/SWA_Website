using Microsoft.Extensions.FileProviders;
using SWA.Api.Common;
using SWA.Application;
using SWA.Application.Common.Content;
using SWA.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton(
    builder.Configuration.GetSection(PublicContentOptions.SectionName).Get<PublicContentOptions>() ?? new PublicContentOptions());

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Media files (icons/images/documents) live on an external file server, keyed by MediaAsset.StorageKey
// (e.g. "2026/07/xxx.webp"), served at /uploads/{storageKey} to match GetMediaByIdQuery's URL construction.
var mediaRoot = builder.Configuration["MediaStorage:RootPath"];
if (!string.IsNullOrWhiteSpace(mediaRoot) && Directory.Exists(mediaRoot))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(mediaRoot),
        RequestPath = "/uploads",
    });
}

app.UseCors("Default");

app.MapControllers();

app.Run();

public partial class Program;
