using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

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

                if (devices == null || !devices.Any())
                {
                    return View("Error", new ErrorViewModel { Message = "No se encontraron dispositivos." });
                }

                var deviceViewModels = devices.Select(DeviceDetailsViewModel.FromDevice).ToList();
                return View(deviceViewModels);
            }
            catch (Services.ServiceException ex)
            {
                _logger.LogWarning(ex, "Error al obtener los dispositivos en el controlador.");
                var errorViewModel = new ErrorViewModel { Message = ex.Message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al obtener dispositivos.");
                var errorViewModel = new ErrorViewModel { Message = "Error de comunicación con el servidor. Por favor, intente más tarde.", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener los dispositivos.");
                var errorViewModel = new ErrorViewModel { Message = "Ocurrió un error inesperado. Por favor, intente más tarde.", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
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

                _logger.LogInformation($"Device data received: {JsonSerializer.Serialize(device)}");

                var deviceDetails = DeviceDetailsViewModel.FromDevice(device);
                Console.WriteLine($"Device Price: {device.Price}");
                return View(deviceDetails);
            }
            catch (Services.ServiceException ex)
            {
                _logger.LogWarning(ex, $"Error al obtener el dispositivo con ID: {id} en el controlador.");
                var errorViewModel = new ErrorViewModel { Message = ex.Message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al obtener el dispositivo.");
                var errorViewModel = new ErrorViewModel { Message = "Error de comunicación con el servidor. Por favor, intente más tarde.", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al obtener el dispositivo con ID: {id}.");
                var errorViewModel = new ErrorViewModel { Message = "Ocurrió un error inesperado. Por favor, intente más tarde.", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                return View("Error", errorViewModel);
            }
        }
    }
}
