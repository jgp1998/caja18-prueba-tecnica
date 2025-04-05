using caja18_prueba_tecnica.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace caja18_prueba_tecnica.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task<IActionResult> Index()
        {
            var devices = await _deviceService.GetDevicesAsync();
            return View(devices);
        }

        public async Task<IActionResult> Details(string id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null) return NotFound();
            return View(device);
        }
    }

}
