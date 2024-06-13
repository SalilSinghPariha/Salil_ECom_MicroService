using AutoMapper;
using Ecom.Services.CoupanAPI;
using Ecom.Services.CoupanAPI.Data;
using Ecom.Services.CoupanAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }) ;

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
        Description="Enter bearer token as folowng: `Bearer Generated-JWT-Token`",
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

builder.AddAppAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db= scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_db.Database.GetPendingMigrations().Count()>=1)
        { 
            _db.Database.Migrate();
        }

    }
}