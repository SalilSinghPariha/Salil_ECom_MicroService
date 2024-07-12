using Microsoft.EntityFrameworkCore;
using Ecom.Service.EmailAPI.Data;
using Ecom.Service.EmailAPI.Messaging;
using Ecom.Service.EmailAPI.Extension;
using Ecom.Service.EmailAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// new singleton db context with email service since we can not go with
// db context with scoped and with singleton spo we need to implement like this 
var optionBuilder= new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));

// automatically start the rabbitmq service so we need to inject it
builder.Services.AddHostedService<RabbitMQAuthConsumer>();
// register RabbitMQCartConsumer
builder.Services.AddHostedService<RabbitMQCartConsumer>();

builder.Services.AddSingleton<IAzureServiceBusConsumer,AzureServiceBusConsumer>();
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

app.UseAuthorization();
app.UseAzureServiceBusConsumer();
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
