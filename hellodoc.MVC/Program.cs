using hellodoc.BAL.Interface;
using hellodoc.BAL.Repository;
using hellodoc.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HellodocDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IPatientReqRepo, PatientReqRepo>();
builder.Services.AddScoped<IPatientDashRepo, PatientDashRepo>();
builder.Services.AddScoped<IRegisterRepo, RegisterRepo>();
builder.Services.AddScoped<IAdminDashboard, AdminDashboard>();
builder.Services.AddScoped<IProviderDashboard, ProviderDashboard>();
builder.Services.AddScoped<IJwtServiceRepo, JwtServiceRepo>();
builder.Services.AddSession();
builder.Services.AddSignalR();

var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Patient/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//clear cache
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRotativa();
app.UseAuthorization();
app.UseSession();

app.MapHub<ChatHub>("/chatHub");

app.MapControllerRoute(
    name: "default",
pattern: "{controller=Patient}/{action=Index}/{id?}");

app.Run();