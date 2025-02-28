using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivePoints.Models
{
    internal class Measurement
    {
        public List<double> I { get; set; }
        public List<double> U { get; set; }
        public double R { get; set; }


        public Measurement(List<double> i, List<double> u, double r)
        {
            I = i;
            U = u;
            R = r;
        }
    }
}
