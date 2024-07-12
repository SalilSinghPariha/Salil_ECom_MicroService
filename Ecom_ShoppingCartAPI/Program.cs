using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Ecom_ShoppingCartAPI.Extensions;
using Ecom_ShoppingCartAPI.Data;
using Ecom_ShoppingCartAPI;
using Ecom_ShoppingCartAPI.Service;
using Ecom_ShoppingCartAPI.Utility;
using Ecom.MessageBus;
using Ecom_ShoppingCartAPI.RabbitMQSender;

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

// Add ProductService
builder.Services.AddScoped<IProductService,ProductService>();
// Add CoupanService
builder.Services.AddScoped<ICoupanService,CoupanService>();
//Add message bus service
//builder.Services.AddScoped<IMessageBus,MessageBus>();
//Now using rabbitmq
builder.Services.AddScoped<IRabbitMQCartSender, RabbitMQCartSender>();

// register delegating hanlder 
builder.Services.AddScoped<BackendAPIAuthenticationHttpClientHandler>();
builder.Services.AddHttpContextAccessor();

// add httpclient so that shopping cart will access productAPI
builder.Services.AddHttpClient("Product", u =>
u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendAPIAuthenticationHttpClientHandler>();

// add httpclient so that shopping cart will access CoupanAPI
builder.Services.AddHttpClient("Coupan", u =>
u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CoupanApi"])).AddHttpMessageHandler<BackendAPIAuthenticationHttpClientHandler>();

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