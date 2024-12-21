using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ActivePoints
{
    /// <summary>
    /// Serial port resource with specified parameters
    /// </summary>
    public class MySerialPort : SerialPort
    {
        //
        public List<string> DataStringBinary = new List<string>();
        public bool isend = false;
        public MySerialPort()
            : base()
        {
            base.BaudRate = 430800;
            base.DataBits = 8;
            base.StopBits = StopBits.One;
            base.Parity = Parity.None;

            base.DataReceived += OnDataReceived;
        }

        public void Open(string portName)
        {
            if (base.IsOpen)
            {
                base.Close();
            }
            base.PortName = portName;
            base.Open();
        }
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var buffer = new byte[serialDevice.BytesToRead];
            serialDevice.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                string sbt = Convert.ToString(buffer[i], 2).PadLeft(8, '0');
                DataStringBinary.Add(sbt);
            }

            if (buffer[^1]==7)
            {
                isend = true;
                base.Close();
            }
        }
    }
}
