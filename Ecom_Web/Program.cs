using Ecom_Web.Services;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add/register accessor/httpclient for coupan and Auth API/addcoupanservice to httpclient and product
//and cart service and order
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICoupanService, CoupanService>();
builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IAuthService,AuthService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
SD.CoupanApiBase = builder.Configuration["ServiceUrl:CoupanApi"];

// for Auth API 
SD.AuthApiBase = builder.Configuration["ServiceUrl:AuthApi"];

//For Product API
SD.ProductApiBase = builder.Configuration["ServiceUrl:ProductApi"];

// for Cart api
SD.CartAPIBase = builder.Configuration["ServiceUrl:CartApi"];

// for Order API
SD.OrderAPIBase = builder.Configuration["ServiceUrl:OrderApi"];

// Register Base and coupan service/AuthService/ITokenProvider
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICoupanService, CoupanService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add Authenitcation using cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options=>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//for Authenication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
