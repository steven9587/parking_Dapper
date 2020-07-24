using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace parking_api.Controllers
{
    public class ParkData
    {
        public float Twd97X { get; set; }
        public float Twd97Y { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        public string ServiceTime { get; set; }
        public string PayEx { get; set; }
        public string TotalCar { get; set; }
        public string TotalMotor { get; set; }
        public string Summary { get; set; }
        public string Id { get; set; }
        public string Tel { get; set; }
        public string UpdateTime { get; set; }
    }
}