using Abp.Net.Mail;
using JewelleryWebApp.Models;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Services;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Azure;
using JewelleryWebApplication.Options;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);



//var connection = String.Empty;
//if (builder.Environment.IsDevelopment())
//{
//    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
//    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
//}
//else
//{
//    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
//}

//builder.Services.AddDbContext<JewelleryWebApplicationContext>(options =>
//    options.UseSqlServer(connection));


var connectionString = builder.Configuration.GetConnectionString("JewelleryWebApplicationContextConnection");
builder.Services.AddDbContext<JewelleryWebApplicationContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<JewelleryWebApplicationContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.Configure<RazorpayClient>(builder.Configuration.GetSection("PaymentSettings"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var service = builder.Services;
service.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));


builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHsts();

    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

   
    var serviceProvider = services.GetRequiredService<IServiceProvider>();
    var configuration = services.GetRequiredService<IConfiguration>();
   //  _context.Database.EnsureCreated();
  //  DbInitializer.SeedUsers(serviceProvider, configuration).Wait();
}
app.UseHttpsRedirection();
app.UseHsts();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();
