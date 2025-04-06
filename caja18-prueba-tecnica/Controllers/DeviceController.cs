using caja18_prueba_tecnica.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace caja18_prueba_tecnica.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var devices = await _deviceService.GetDevicesAsync();
                return View(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los dispositivos en el controlador.");
                ViewBag.ErrorMessage = "Ocurrió un error al obtener los dispositivos. Por favor, intente más tarde.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    return NotFound();
                }
                return View(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el dispositivo con ID: {id} en el controlador.");
                ViewBag.ErrorMessage = "Ocurrió un error al obtener el dispositivo. Por favor, intente más tarde.";
                return View("Error");
            }
        }
    }
}
