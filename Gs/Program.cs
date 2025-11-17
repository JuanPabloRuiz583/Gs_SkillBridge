using System.Reflection;
using System.Text;

using Gs.Data;
using Gs.Services;

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 🔹 Logging básico (console)
// =========================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// =========================
// 🔹 HTTP Logging (Tracing simples)
// =========================
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestHeaders |
        HttpLoggingFields.ResponseStatusCode;
});

// =========================
// 🔹 Banco de Dados Oracle
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// 🔹 Controllers / Endpoints
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =========================
// 🔹 CORS (igual MottuSense)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =========================
// 🔹 Injeção de dependências
// =========================
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();


// =========================
// 🔹 Health Checks
// =========================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("oracle")
    .AddCheck("self", () => HealthCheckResult.Healthy("API está rodando!"));

// 🔹 Health Checks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetHeaderText("SkillBridge Health Checks UI");
    options.AddHealthCheckEndpoint("API Health", "/health");
}).AddInMemoryStorage();

// =========================
// 🔹 JWT
// =========================
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? "minha-chave-super-secreta-1234567890";

var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Logs de JWT (ajuda muito em 401)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine("==== JWT OnMessageReceived ====");
                Console.WriteLine("Path.........: " + context.Request.Path);
                Console.WriteLine("Authorization: " + context.Request.Headers["Authorization"]);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("⚠️ ERRO JWT: " + context.Exception.GetType().Name);
                Console.WriteLine(context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// =========================
// 🔹 Swagger + JWT
// =========================
builder.Services.AddSwaggerGen(configurationSwagger =>
{
    configurationSwagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API SkillBridge",
        Version = "v1",
        Description = "API de gerenciamento de clientes e vagas\r\n> SOLID\r\n> CleanCode\r\n> OpenAPI (Swashbuckle)\r\n",
        Contact = new OpenApiContact
        {
            Name = "juan"
        }
    });

    configurationSwagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Cole APENAS o token JWT (sem a palavra 'Bearer'). " +
                      "O header será enviado como: Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    configurationSwagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    configurationSwagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// =========================
// 🔹 Pipeline HTTP
// =========================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API SkillBridge v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});

app.UseHttpsRedirection();

// 🔹 Tracing de requisição/resposta HTTP
app.UseHttpLogging();

// CORS antes da autenticação/autorização
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint de health check detalhado (JSON)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Endpoint da interface gráfica do health check
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.Run();
