using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ActivePoints.ViewModels
{
    class StartMeasurementViewModel : ViewModelBase
    {
        private string[] avaliblePorts;
        public string[] AvaliblePorts
        {
            get { return avaliblePorts; }
            set { 
                avaliblePorts = value;
                OnPropertyChanged(nameof(avaliblePorts));
            }
        }
        private string chosenPort;
        public string ChosenPort
        {
            get { return chosenPort; }
            set { chosenPort = value; }
        }
        public ICommand StartNewMeasurmentCommand { get; }

    }
}
