using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Telegram.Bot;

using Presentation.Common;
using Presentation.Telegram;
using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Configurations;
using Presentation.Middlewares;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddCors(
    option => option.AddDefaultPolicy(
        builder => builder
            .AllowAnyMethod().AllowAnyHeader()
            .AllowCredentials().SetIsOriginAllowed(origin => true)));

builder.Services.Configure<TokenConfiguration>(
    configuration.GetSection("JsonWebToken"));

builder.Services.Configure<TelegramConfiguration>(
    configuration.GetSection("Telegram"));

var configurationJWT = configuration.GetSection("JsonWebToken")
    .Get<TokenConfiguration>();

var configurationTelegram = configuration.GetSection("Telegram")
    .Get<TelegramConfiguration>();

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        TelegramBotClientOptions options = new(configurationTelegram.Token);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandler>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddTransient<UnitOfWork>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<ApplicationDBContext>(
    x => x.UseMySql(configuration.GetConnectionString("MySqlConnection"), new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configurationJWT.Issuer,
        ValidAudience = configurationJWT.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configurationJWT.Secret)),
    };
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();

