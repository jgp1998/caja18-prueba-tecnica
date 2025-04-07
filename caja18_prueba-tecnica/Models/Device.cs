using System.Text.Json;

namespace caja18_prueba_tecnica.Models
{
    public class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object>? Data { get; set; }

        public string? Color => GetDataValue<string>("color");
        
        public string? Capacity
        {
            get
            {
                var capacity = GetDataValue<string>("capacity");
                if (!string.IsNullOrEmpty(capacity)) return capacity;

                var capacityGB = GetDataValue<string>("Capacity GB");
                if (!string.IsNullOrEmpty(capacityGB)) return $"{capacityGB} GB";

                var capacityValue = GetDataValue<decimal>("Capacity");
                if (capacityValue > 0) return $"{capacityValue} GB";

                return null;
            }
        }

        public decimal? Price
        {
            get
            {
                var price = GetDataValue<decimal>("price");
                return price > 0 ? price : null;
            }
        }

        public string? Generation
        {
            get
            {
                var generation = GetDataValue<string>("generation");
                if (!string.IsNullOrEmpty(generation)) return generation;

                // Intentar con diferentes formatos de la clave
                generation = GetDataValue<string>("Generation");
                if (!string.IsNullOrEmpty(generation)) return generation;

                generation = GetDataValue<string>("gen");
                if (!string.IsNullOrEmpty(generation)) return generation;

                return null;
            }
        }

        public string? CpuModel => GetDataValue<string>("CPU model");
        public string? HardDiskSize => GetDataValue<string>("Hard disk size");
        public string? StrapColour => GetDataValue<string>("Strap Colour");
        public string? CaseSize => GetDataValue<string>("Case Size");
        public string? Description => GetDataValue<string>("Description");
        public string? ScreenSize => GetDataValue<string>("Screen size");

        private T? GetDataValue<T>(string key)
        {
            if (Data == null || !Data.ContainsKey(key))
                return default;

            try
            {
                var value = Data[key];
                if (value == null)
                    return default;

                if (value is T typedValue)
                    return typedValue;

                if (value is JsonElement jsonElement)
                {
                    return jsonElement.ValueKind switch
                    {
                        JsonValueKind.String => (T)(object)jsonElement.GetString(),
                        JsonValueKind.Number => (T)(object)jsonElement.GetDecimal(),
                        JsonValueKind.True => (T)(object)true,
                        JsonValueKind.False => (T)(object)false,
                        _ => default
                    };
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        public Dictionary<string, string> GetAllProperties()
        {
            var properties = new Dictionary<string, string>
            {
                { "Id", Id },
                { "Name", Name }
            };

            if (Data != null)
            {
                foreach (var item in Data)
                {
                    var normalizedKey = NormalizeKey(item.Key);
                    if (!properties.ContainsKey(normalizedKey))
                    {
                        var value = item.Value?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            properties[normalizedKey] = value;
                        }
                    }
                }
            }

            return properties;
        }

        private string NormalizeKey(string key)
        {
            return key.ToLower() switch
            {
                "capacity" or "capacity gb" or "capacitygb" => "Capacity",
                "color" or "colour" => "Color",
                "hard disk size" or "harddisksize" => "Hard Disk Size",
                "cpu model" or "cpumodel" => "CPU Model",
                "screen size" or "screensize" => "Screen Size",
                "strap colour" or "strapcolor" => "Strap Color",
                "case size" or "casesize" => "Case Size",
                "generation" or "gen" => "Generation",
                _ => key
            };
        }
    }
} 