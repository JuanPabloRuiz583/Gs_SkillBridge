using System.Reflection;
using System.Text;

using Gs.Data;
using Gs.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 🔹 Banco de Dados Oracle
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// 🔹 Injeção de Dependências
// =========================
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

// =========================
// 🔹 Configuração JWT
// =========================
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Jwt:Key não configurado em appsettings.json.");

var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // em produção, ideal: true com HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateIssuer = false,
            ValidateAudience = false,

            // por padrão o ValidateLifetime é true
            // Se quiser TESTAR ignorando expiração, descomente a linha abaixo:
            // ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        // 🔍 Loga o motivo exato do token inválido no console
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("⚠️ ERRO JWT: " + context.Exception.GetType().Name);
                Console.WriteLine(context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// =========================
// 🔹 Swagger + JWT (cadeado)
// =========================
builder.Services.AddSwaggerGen(configurationSwagger =>
{
    configurationSwagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API SkillBridge",
        Version = "v1",
        Description = "Uma API simples de gerenciamento\r\n> + SOLID\r\n+ CleanCode\r\n+ Documentação com a OpenAPI (Swashbuckle)\r\n",
        Contact = new OpenApiContact
        {
            Name = "juan"
        }
    });

    configurationSwagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Cole APENAS o token JWT (sem a palavra 'Bearer'). O header será enviado como: Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,   // Http + Bearer -> Swagger adiciona o 'Bearer ' pra você
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
app.UseSwaggerUI(configurationSwagger =>
{
    configurationSwagger.SwaggerEndpoint("/swagger/v1/swagger.json", "API SkillBridge v1");
    configurationSwagger.RoutePrefix = string.Empty; // Swagger UI como página inicial
});

app.UseHttpsRedirection();

app.UseAuthentication(); // 🔐 JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
