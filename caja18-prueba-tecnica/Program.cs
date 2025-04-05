using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Repositories;
using caja18_prueba_tecnica.Services.Interfaces;
using caja18_prueba_tecnica.Services;

var builder = WebApplication.CreateBuilder(args);
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// Add services to the container.
builder.Services.AddControllersWithViews();
if (string.IsNullOrEmpty(baseUrl))
    throw new InvalidOperationException("API BaseUrl is missing in configuration.");
builder.Services.AddHttpClient<IDeviceRepository, DeviceRepository>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});


builder.Services.AddScoped<IDeviceService, DeviceService>();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Device}/{action=Index}/{id?}");

app.Run();
