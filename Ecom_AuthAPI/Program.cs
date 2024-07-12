using Ecom.MessageBus;
using Ecom.Services.AuthAPI.Data;
using Ecom_AuthAPI.Models;
using Ecom_AuthAPI.Models.Dto;
using Ecom_AuthAPI.RabbitMQSender;
using Ecom_AuthAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options=>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

//Register AuthService
builder.Services.AddScoped<IAuthService, AuthService> ();
//Inject message bus for  publish message
//builder.Services.AddScoped<IMessageBus, MessageBus>();
//Inject message bus for  publish message
builder.Services.AddScoped<IRabbitMQAuthSender, RabbitMQAuthSender>();

//for jwt token
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//Add jwt option from configuration and configure it in JwtOptions

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSetting:JwtOptions"));

//Add identity and here appliactionuser since we extended this from same identityuser only for custom properity
//so we need to  have application user here while adding identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope= app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (db.Database.GetPendingMigrations().Count() >= 1)
        {
            db.Database.Migrate();
        }

    }
}
