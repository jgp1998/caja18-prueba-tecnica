using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Services;
using caja18_prueba_tecnica.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace caja18_prueba_tecnica.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;
        private readonly ILogger<DeviceService> _logger;
        private readonly IMemoryCache _cache;
        private const string DevicesCacheKey = "cached_devices";

        public DeviceService(
            IDeviceRepository repository,
            ILogger<DeviceService> logger,
                        IMemoryCache cache
            )
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
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
                else
                {
                    cachedDevices = Enumerable.Empty<Device>(); 
                }


                var devices = await _repository.GetAllAsync();

                if (devices == null || !devices.Any())
                {
                    throw new ServiceException("No se encontraron dispositivos.", HttpStatusCode.NotFound);
                }

                _cache.Set(DevicesCacheKey, devices, TimeSpan.FromHours(24));

                return devices;
            }
            catch (ServiceException ex)
            {
                _logger.LogWarning(ex, "Error al obtener dispositivos en la capa de servicio.");
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al intentar obtener dispositivos.");
                throw new ServiceException("Error de comunicación con el servidor. Intente nuevamente más tarde.", HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener dispositivos.");
                throw new ServiceException("Ocurrió un error inesperado. Intente nuevamente más tarde.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Device?> GetDeviceByIdAsync(string id)
        {
            try
            {
                var device = await _repository.GetByIdAsync(id);

                if (device == null)
                {
                    throw new ServiceException($"No se encontró el dispositivo con ID: {id}.", HttpStatusCode.NotFound);
                }

                return device;
            }
            catch (ServiceException ex)
            {
                _logger.LogWarning(ex, $"Error al obtener el dispositivo con ID: {id} en la capa de servicio. Detalles: {ex.Message}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al intentar obtener el dispositivo.");
                throw new ServiceException("Error de comunicación con el servidor. Intente nuevamente más tarde.", HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al obtener el dispositivo con ID: {id}.");
                throw new ServiceException("Ocurrió un error inesperado. Intente nuevamente más tarde.", HttpStatusCode.InternalServerError);
            }
        }
    }


}
