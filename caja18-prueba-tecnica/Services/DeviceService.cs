using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Services.Interfaces;

namespace caja18_prueba_tecnica.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;

        public DeviceService(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Device>> GetDevicesAsync() => _repository.GetAllAsync();
        public Task<Device?> GetDeviceByIdAsync(string id) => _repository.GetByIdAsync(id);
    }

}
