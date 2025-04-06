using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Services.Interfaces;

namespace caja18_prueba_tecnica.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;
        private readonly ILogger<DeviceService> _logger;
        public DeviceService(IDeviceRepository repository, ILogger<DeviceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dispositivos en la capa de servicio.");
                throw new Exception("Error al obtener dispositivos. Intente nuevamente más tarde.");
            }
        }

        public async Task<Device?> GetDeviceByIdAsync(string id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el dispositivo con ID: {id} en la capa de servicio.");
                throw new Exception($"Error al obtener el dispositivo con ID: {id}. Intente nuevamente más tarde.");
            }
        }
    }

}
