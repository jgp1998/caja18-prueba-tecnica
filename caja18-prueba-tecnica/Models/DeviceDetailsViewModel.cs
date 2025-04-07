using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace caja18_prueba_tecnica.Models
{
    public class DeviceDetailsViewModel
    {
        [Display(Name = "ID")]
        public string? Id { get; set; }

        [Display(Name = "Nombre")]
        public string? Name { get; set; }

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Capacidad")]
        public string? Capacity { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public string? Price { get; set; }

        [Display(Name = "Generación")]
        public string? Generation { get; set; }

        [Display(Name = "Año")]
        public string? Year { get; set; }

        [Display(Name = "Modelo de CPU")]
        public string? CpuModel { get; set; }

        [Display(Name = "Tamaño del Disco Duro")]
        public string? HardDiskSize { get; set; }

        [Display(Name = "Color de la Correa")]
        public string? StrapColour { get; set; }

        [Display(Name = "Tamaño de la Caja")]
        public string? CaseSize { get; set; }

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Tamaño de Pantalla")]
        public string? ScreenSize { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; } = new();

        public static DeviceDetailsViewModel FromDevice(Device device)
        {
            var viewModel = new DeviceDetailsViewModel
            {
                Id = device.Id,
                Name = device.Name,
                Color = device.Color,
                Capacity = device.Capacity,
                Price = device.Price,
                Generation = device.Generation,
                Year = device.Year,
                CpuModel = device.CPUModel,
                HardDiskSize = device.HardDiskSize,
                StrapColour = device.StrapColour,
                CaseSize = device.CaseSize,
                Description = device.Description,
                ScreenSize = device.ScreenSize
            };

            var allProperties = device.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .ToDictionary(p => p.Name, p => p.GetValue(device)?.ToString());

            var viewModelProps = typeof(DeviceDetailsViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var (key, value) in allProperties)
            {
                var matchingProp = viewModelProps.FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (matchingProp == null)
                {
                    viewModel.AdditionalProperties[key] = value;
                    continue;
                }

                var currentValue = matchingProp.GetValue(viewModel)?.ToString();
                if (!string.Equals(currentValue, value, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var convertedValue = ConvertValue(value, matchingProp.PropertyType);
                        matchingProp.SetValue(viewModel, convertedValue);
                    }
                    catch
                    {
                        viewModel.AdditionalProperties[key] = value;
                    }
                }
            }

            return viewModel;
        }

        private static object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (targetType == typeof(string)) return value;

            if (targetType == typeof(decimal?) && decimal.TryParse(value, out var d))
                return d;

            if (targetType == typeof(int?) && int.TryParse(value, out var i))
                return i;

            if (Nullable.GetUnderlyingType(targetType) is Type innerType)
                return Convert.ChangeType(value, innerType);

            return Convert.ChangeType(value, targetType);
        }
    }
}
