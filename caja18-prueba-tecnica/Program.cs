using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Repositories;
using caja18_prueba_tecnica.Services.Interfaces;
using caja18_prueba_tecnica.Services;

var builder = WebApplication.CreateBuilder(args);

var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrEmpty(baseUrl))
{
    builder.Services.AddSingleton<ILogger<DeviceRepository>>(serviceProvider =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<DeviceRepository>>();
        logger.LogError("La URL de la API no está configurada correctamente en 'appsettings.json'.");
        return logger;
    });
    throw new InvalidOperationException("API BaseUrl is missing in configuration.");
}
 
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IDeviceRepository, DeviceRepository>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<IDeviceService, DeviceService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
