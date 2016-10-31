using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class WelcomeViewModel : ViewModelBase
    {
        public WelcomeViewModel()
        {
            LastCircuitsList = new ObservableCollection<string>();
        }

        public ObservableCollection<string> LastCircuitsList { get; }
    }
}