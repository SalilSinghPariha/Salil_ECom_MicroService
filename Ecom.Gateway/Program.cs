using Ecom.Gateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json",optional:false,reloadOnChange:true); // optional false measn it's required on and onreaload true
// add ocelot.json while AddOcelot
builder.Services.AddOcelot(builder.Configuration);
builder.AddAppAuthentication();
var app = builder.Build();
app.UseOcelot().GetAwaiter().GetResult();
app.MapGet("/", () => "Hello World!");

app.Run();
