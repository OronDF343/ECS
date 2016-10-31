using System.Collections.ObjectModel;
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