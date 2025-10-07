using Dapper;
using Dommel;
using ReactApp1.Server.Apresentacao.Dependencias;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.Negocio.Servicos;
using ReactApp1.Server.Negocio.Servicos.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- CONFIGURAÇÃO DE SERVIÇOS ---

// 2. Unit of Work
builder.Services.AddScoped<IUnitOfWork>(provider =>
    new UnitOfWork(connectionString!));

// 3. Serviços de Negócio
builder.Services.AddScoped<IMaterialServico, MaterialServico>();
builder.Services.AddScoped<ITracoServico, TracoServico>();

//4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        // aceita qualquer origem cujo host seja 'localhost' ou '127.0.0.1' (qualquer porta)
        policy.SetIsOriginAllowed(origin =>
        {
            try
            {
                var uri = new Uri(origin);
                return uri.Host == "localhost" || uri.Host == "127.0.0.1";
            }
            catch
            {
                return false;
            }
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

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

// Aplicar CORS para permitir requisições do frontend em localhost
app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
