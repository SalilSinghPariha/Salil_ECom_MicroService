using AutoMapper;
using Ecom.Service.ProductAPI.Data;
using Microsoft.EntityFrameworkCore;
using Ecom.Service.ProductAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// inject mapper to builder service
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);

// Add Automapper to so that we can use it through dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

// Authorization using JWT Token
builder.Services.AddSwaggerGen(Options =>
{
    //Add Authorization Header
    Options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter bearer token as folowng: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    //Add Security Requirment
    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                },

                Scheme="Oauth2",
                Name= JwtBearerDefaults.AuthenticationScheme,
                In= ParameterLocation.Header

            },

            new List<string>()
        }
    });
});


var Secret = builder.Configuration.GetValue<string>("JwtSetting:Secret");
var Issuer = builder.Configuration.GetValue<string>("JwtSetting:Issuer");
var Audience = builder.Configuration.GetValue<string>("JwtSetting:Audience");

var key = Encoding.ASCII.GetBytes(Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {

    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = Issuer,
        ValidAudience = Audience,
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//to add below app will go to wwroot folder and use statuc file and fetch it for image
app.UseStaticFiles();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_db.Database.GetPendingMigrations().Count() >= 1)
        {
            _db.Database.Migrate();
        }

    }
}