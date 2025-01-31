using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using restaurante_C_api.Data;
using restaurante_C_api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=sql,1440;Database=RestauranteDB;User Id=sa;Password=123G3FJJMA.;Encrypt=True;TrustServerCertificate=True;";

// Registo do DbContext e de Serviços
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IReservaService, ReservaService>();

// Serviços de controladores e Swagger
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurante API Grupo C",
        Version = "v1",
        Description = "API para gestão de reservas de restaurante, permitindo operações como criar, atualizar, listar e excluir reservas feita por Asafe, Fardeen , João , Júlio e Miguel.",
        Contact = new OpenApiContact
        {
            Name = "Grupo C",
            Email = "grupoC@restaurante.com",
            Url = new Uri("https://www.isep.ipp.pt/")
        }
    });
});

var app = builder.Build();

// Config pipeline http
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
