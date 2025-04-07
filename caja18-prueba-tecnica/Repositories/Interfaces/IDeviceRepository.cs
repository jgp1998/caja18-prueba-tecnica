using caja18_prueba_tecnica.Models;

namespace caja18_prueba_tecnica.Repositories.Interfaces
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAllAsync();
        Task<Device?> GetByIdAsync(string id);
        Device MapToDevice(Dictionary<string, object> data);
    }
}
