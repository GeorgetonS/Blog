using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
LoadConfiguration(app);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.MapControllers();
app.UseStaticFiles();

/*if (app.Environment.IsProduction())
{
    Console.WriteLine("Ambiente de produção.");
}
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Ambiente de desenvolvimento");
}*/


if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Ambiente de desenvolvimento");
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();

void LoadConfiguration(WebApplication app)
{
  
    Configuration.JwtKey = app.Configuration.GetValue<string>(key: "JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>(key: "ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>(key: "ApiKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection(key: "Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCompression(options =>
    {
        // options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        // options.Providers.Add<CustomCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    }); ;
}

void ConfigureServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<BlogDataContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();


    //builder.Services.AddTransient<TokenService>();//sempre cria um novo
    //builder.Services.AddScoped(); //por transação
    //builder.Services.AddSingleton();// 1 por appp 
}