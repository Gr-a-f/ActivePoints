using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading.Tasks;
using ScottPlot;

namespace ActivePoints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public delegate void OnOneMessureEnded(List<string> dataStringBinary);
    public partial class MainWindow : Window
    {
        public string selectedport;
        public MainWindow()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            COMbox.ItemsSource = ports;
            if (ports.Length > 0)
            {
                COMbox.SelectedIndex = 0;
            }
        }

        public void DataProcessing(List<string> Data, ref List<double> myI, ref List<double> myU, ref List<double> myx)
        {
            //Dividing the data into current and voltage arrays
            var BinString = Data;
            var i0 = new List<string>();
            var u0 = new List<string>();

            try
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    if (BinString[i].Substring(0, 4) == "0100")
                    {
                        i0.Add(BinString[i].Substring(4) + BinString[i + 1]
                            + BinString[i + 2] + BinString[i + 3]
                            + BinString[i + 4]);
                    }
                    else if (BinString[i].Substring(0, 4) == "0110")
                    {
                        u0.Add(BinString[i].Substring(4) + BinString[i + 1]
                            + BinString[i + 2] + BinString[i + 3]
                            + BinString[i + 4]);
                    }
                }


                //Conversion of data into numerical format
                string ubin;
                string ibin;

                for (int i = 0; i < i0.Count; i++)
                {
                    ibin = i0[i][0] + i0[i].Substring(5, 7)
                        + i0[i][1] + i0[i].Substring(13, 7)
                        + i0[i][2] + i0[i].Substring(21, 7)
                        + i0[i][3] + i0[i].Substring(29, 7);
                    myI.Add(Convert.ToInt32(ibin, 2));
                    myx.Add(i);
                }

                for (int i = 0; i < u0.Count; i++)
                {
                    ubin = u0[i][0] + u0[i].Substring(5, 7)
                        + u0[i][1] + u0[i].Substring(13, 7)
                        + u0[i][2] + u0[i].Substring(21, 7)
                        + u0[i][3] + u0[i].Substring(29, 7);
                    myU.Add(Convert.ToInt32(ubin, 2));
                }
            }
            catch 
            {
                //Errors in this section usually occur when the electrode is pressed several times
                //during the measurement without waiting for the measurement to complete.
                Dispatcher.Invoke(new Action(() => TMstatus.Text = "Data transmission error. Repeat measurement"));
            }
        }

        private void Bstart_Click(object sender, RoutedEventArgs e)
        {
            if (COMbox.SelectedValue == null)
            {
                Dispatcher.Invoke(new Action(() => TMstatus.Text = "COM-port not detected"));
            }
            else
            {
                Bstart.IsEnabled = false;
                Dispatcher.Invoke(new Action(() => TMstatus.Text = "Measurement started"));
                selectedport = COMbox.SelectedValue.ToString();
                Thread task = new Thread(() => Meashure());
                task.Start();
            }
        }
        private void Meashure()
        {
            OnOneMessureEnded? mes;
            mes = EndMes;
            var port = new MySerialPort(mes);
            port.Open("COM3");
            ChangeStatus("Calibration");
            port.Calibrate();
            ChangeStatus("Calibration Ended");
        }
        public void ChangeStatus(string text)
        {
            Dispatcher.Invoke(new Action(() => TMstatus.Text = text));
        }
        public void EndMes(List<string> dataStringBinary)
        {
            List<double> I = new List<double>();
            List<double> U = new List<double>();
            List<double> x = new List<double>();
            DataProcessing(dataStringBinary, ref I, ref U,ref x);

            ResultU.Plot.Clear();
            ResultI.Plot.Clear();
            ResultI.Plot.Add.Scatter(x, I);
            ResultU.Plot.Add.Scatter(x, U);
            ResultI.Plot.Axes.AutoScale();
            ResultU.Plot.Axes.AutoScale();
            ResultU.Refresh();
            ResultI.Refresh();
        }

        void OnDropDownOpened(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            COMbox.ItemsSource = ports;
        }
    }
}