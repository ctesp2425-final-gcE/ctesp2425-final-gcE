using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using restaurante_C_api.Data;
using restaurante_C_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar string de conexão diretamente no código
var connectionString = "Server=192.168.50.4;Database=RestauranteDB;User Id=gc;Password=123G3FJJMA.;";

// Registrar o DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar serviços
builder.Services.AddScoped<IReservaService, ReservaService>();

// Adicionar serviços de controladores
builder.Services.AddControllers();

// Adicionar serviços para o Swagger
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

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
    });
}

app.UseHttpsRedirection();

// Configurar endpoints de controladores
app.MapControllers();

app.Run();
