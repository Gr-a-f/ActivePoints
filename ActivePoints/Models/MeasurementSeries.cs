using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivePoints.Models
{
    internal class MeasurementSeries
    {
        public MeasurementSeries(List<Measurement> measurements, string user, DateTime data)
        {
            Measurements = measurements;
            User = user;
            Data = data;
        }

        public List<Measurement> Measurements { get; set; }
        public string User { get; set; }
        public DateTime Data { get; set; }
    }
}
