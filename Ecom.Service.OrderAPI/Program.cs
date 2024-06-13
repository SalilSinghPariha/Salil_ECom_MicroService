using Microsoft.EntityFrameworkCore;
using Ecom.Service.OrderAPI.Data;
using AutoMapper;
using Ecom.Service.OrderAPI;
using Ecom.Service.OrderAPI.Service;
using Ecom.Service.OrderAPI.Extensions;
using Ecom.MessageBus;
using Ecom.Service.OrderAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Stripe;

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

builder.Services.AddScoped<IProductServicecs, Ecom.Service.OrderAPI.Service.ProductService>();

builder.Services.AddScoped<IMessageBus, MessageBus>();

// register delegating hanlder 
builder.Services.AddScoped<BackendAPIAuthenticationHttpClientHandler>();
builder.Services.AddHttpContextAccessor();

// add httpclient so that shopping cart will access productAPI
builder.Services.AddHttpClient("Product", u =>
u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendAPIAuthenticationHttpClientHandler>();

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


builder.AddAppAuthentication();

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
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_db.Database.GetPendingMigrations().Count() >= 1)
        {
            _db.Database.Migrate();
        }

    }
}
