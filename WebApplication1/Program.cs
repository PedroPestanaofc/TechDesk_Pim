using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TechDesk.Data;
using TechDesk.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechDesk",
        Version = "v1",
        Description = "API de Suporte Técnico - TechDesk"
    });
});

builder.Services.AddDbContext<TechDeskDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<TechDesk.Services.MensagemService>();
builder.Services.AddScoped<TechDesk.Repositories.MensagemRepository>();
builder.Services.AddHttpClient<IaService>();


var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
