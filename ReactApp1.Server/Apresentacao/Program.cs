using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.Negocio.Servicos;
using ReactApp1.Server.Negocio.Servicos.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- CONFIGURAÇÃO DE SERVIÇOS ---

// 1. DbContext
builder.Services.AddDbContext<SuperMixDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Injeção de Dependência
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMaterialServico, MaterialServico>();
builder.Services.AddScoped<ITracoServico, TracoServico>();


// 5. Serviços Padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- BUILD DA APLICAÇÃO ---
var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
