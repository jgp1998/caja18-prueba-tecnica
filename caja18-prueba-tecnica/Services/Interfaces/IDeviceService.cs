using caja18_prueba_tecnica.Models;

namespace caja18_prueba_tecnica.Services.Interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetDevicesAsync();
        Task<Device?> GetDeviceByIdAsync(string id);
    }

}
