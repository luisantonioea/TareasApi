using Microsoft.EntityFrameworkCore;
using TareasApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuración de SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar soporte para Controladores
builder.Services.AddControllers();

// Configuración de IHttpClientFactory para consumir la API externa
builder.Services.AddHttpClient("JsonPlaceholder", client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

// Configuración de Swagger (Documentación de la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear los endpoints de tus controladores
app.MapControllers(); 

app.Run();