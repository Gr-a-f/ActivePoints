using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivePoints
{
    internal class OneMesureData
    {
        private double[] RawData{ get; set; }
        private double[] I;
        private double[] U;

        public delegate void AccountHandler(string message);
        public event AccountHandler? Notify;
    }
}
