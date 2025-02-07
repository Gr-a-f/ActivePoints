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
    public partial class MainWindow : Window
    {
        // There should be a total of 1953 points after all the transformations
        public double[] myI = new double[1953];
        public double[] myU = new double[1953];
        public double[] myx = new double[1953];
        public string selectedport;
        public MySerialPort chosenport;

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

        public void DataProcessing(List<string> Data, ref double[] myI, ref double[] myU, ref double[] myx)
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
                    myI[i] = Convert.ToInt32(ibin, 2);
                    myx[i] = i;
                }

                for (int i = 0; i < u0.Count; i++)
                {
                    ubin = u0[i][0] + u0[i].Substring(5, 7)
                        + u0[i][1] + u0[i].Substring(13, 7)
                        + u0[i][2] + u0[i].Substring(21, 7)
                        + u0[i][3] + u0[i].Substring(29, 7);
                    myU[i] = Convert.ToInt32(ubin, 2);
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
                Dispatcher.Invoke(new Action(() => TMstatus.Text = "Measurement started"));
                selectedport = COMbox.SelectedValue.ToString();
                Task task = new Task(() => Meashure());
                task.Start();
            }
        }
        private void Meashure()
        {
            var port = new MySerialPort();
            port.Open(selectedport);
            while (!port.isend)
            {
            }

            DataProcessing(port.DataStringBinary, ref myI, ref myU, ref myx);
            Plot();

            Dispatcher.Invoke(new Action(() => TMstatus.Text = "Measurement completed"));
        }
        private void Plot()
        {
            ResultU.Plot.Clear();
            ResultI.Plot.Clear();
            ResultI.Plot.Add.Scatter(myx, myI);
            ResultU.Plot.Add.Scatter(myx, myU);
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