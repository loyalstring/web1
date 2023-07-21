using Abp.Net.Mail;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Azure;
using JewelleryWebApplication.Options;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
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
builder.Services.AddAuthentication()
        .AddFacebook(options =>
        {
            options.AppId = "806967584353963";
            options.AppSecret = "b3f679e858b9ed5345fb48315e6fd220";
        });
//builder.Services.Configure<RazorpayClient>(builder.Configuration.GetSection("PaymentSettings"));
builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = "967239436655-u2vnmgm26o36cmltr5u33hovnv93b84l.apps.googleusercontent.com";
            options.ClientSecret = "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";
        });
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
    
var service = builder.Services;
service.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));


//builder.Services.AddSwaggerGen();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();
