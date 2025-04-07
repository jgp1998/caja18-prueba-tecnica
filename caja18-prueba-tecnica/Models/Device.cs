using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace caja18_prueba_tecnica.Models
{
    public class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Capacity { get; set; }
        public string Price { get; set; }  
        public string Description { get; set; }
        public string Year { get; set; }
        public string CPUModel { get; set; }
        public string HardDiskSize { get; set; }
        public string ScreenSize { get; set; }
        public string Generation { get; set; }
        public string StrapColour { get; set; }
        public string CaseSize { get; set; }
    }
}
