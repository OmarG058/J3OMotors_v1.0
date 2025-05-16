using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using J3OMotors_v1._0.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<J3OMotors_v1_0Context>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("J3OMotors_v1_0Context") ?? throw new InvalidOperationException("Connection string 'J3OMotors_v1_0Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
//Configurar cliente HTTP
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();




builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo que dura la sesi�n
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
