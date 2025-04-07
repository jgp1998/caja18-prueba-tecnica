using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Services;
using caja18_prueba_tecnica.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.RegularExpressions;

namespace caja18_prueba_tecnica.Services
{
    public enum ServiceErrorCode
    {
        NotFound = 404,
        BadRequest = 400,
        InternalError = 500,
        ServiceUnavailable = 503
    }

    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;
        private readonly ILogger<DeviceService> _logger;
        private readonly IMemoryCache _cache;
        private const string DevicesCacheKey = "cached_devices";
        private const string DeviceCacheKeyPrefix = "device_";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);

        public DeviceService(
            IDeviceRepository repository,
            ILogger<DeviceService> logger,
            IMemoryCache cache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private void HandleException(Exception ex, string message, string? id = null)
        {
            if (ex is ServiceException serviceEx)
            {
                _logger.LogWarning(serviceEx, $"{message} (ID: {id ?? "N/A"})");
                throw serviceEx;
            }
            else if (ex is HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"{message} (ID: {id ?? "N/A"})");
                throw new ServiceException("Error de comunicación con el servidor. Intente nuevamente más tarde.", HttpStatusCode.ServiceUnavailable);
            }
            else
            {
                _logger.LogError(ex, $"{message} (ID: {id ?? "N/A"})");
                throw new ServiceException("Ocurrió un error inesperado. Intente nuevamente más tarde.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            try
            {
                if (_cache.TryGetValue(DevicesCacheKey, out IEnumerable<Device> cachedDevices) && cachedDevices != null)
                {
                    _logger.LogInformation("Dispositivos obtenidos desde el caché.");
                    return cachedDevices;
                }

                _logger.LogInformation("Obteniendo dispositivos desde el repositorio.");
                var devices = await _repository.GetAllAsync();

                if (devices == null || !devices.Any())
                {
                    _logger.LogWarning("No se encontraron dispositivos en el repositorio.");
                    throw new ServiceException("No se encontraron dispositivos.", HttpStatusCode.NotFound);
                }

                _logger.LogInformation("Almacenando {Count} dispositivos en caché.", devices.Count());
                _cache.Set(DevicesCacheKey, devices, CacheExpiration);

                return devices;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error al obtener dispositivos");
                return Enumerable.Empty<Device>();
            }
        }

        public async Task<Device?> GetDeviceByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Se intentó obtener un dispositivo con ID nulo o vacío.");
                throw new ServiceException("El ID del dispositivo no puede estar vacío.", HttpStatusCode.BadRequest);
            }

            try
            {
                var cacheKey = $"{DeviceCacheKeyPrefix}{id}";
                if (_cache.TryGetValue(cacheKey, out Device cachedDevice) && cachedDevice != null)
                {
                    _logger.LogInformation("Dispositivo con ID {Id} obtenido desde el caché.", id);
                    return cachedDevice;
                }

                _logger.LogInformation("Obteniendo dispositivo con ID {Id} desde el repositorio.", id);
                var device = await _repository.GetByIdAsync(id);

                if (device == null)
                {
                    _logger.LogWarning("No se encontró el dispositivo con ID {Id}.", id);
                    throw new ServiceException($"No se encontró el dispositivo con ID: {id}.", HttpStatusCode.NotFound);
                }

                _logger.LogInformation("Almacenando dispositivo con ID {Id} en caché.", id);
                _cache.Set(cacheKey, device, CacheExpiration);

                return device;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error al obtener el dispositivo con ID", id);
                return null; 
            }
        }
    }
}
