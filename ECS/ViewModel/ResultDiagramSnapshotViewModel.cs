using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ECS.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ECS.ViewModel
{
    public class ResultDiagramSnapshotViewModel : ViewModelBase
    {
        public ResultDiagramSnapshotViewModel(IEnumerable<DiagramObject> cir, string name)
        {
            Diagram = new ObservableCollection<DiagramObject>();
            Name = name;
            _circuit = cir;
        }

        private IEnumerable<DiagramObject> _circuit;
        public ObservableCollection<DiagramObject> Diagram { get; }
        public string Name { get; }
        public ICommand LoadedCommand => new RelayCommand(Loaded);

        private void Loaded()
        {
            if (_circuit != null) foreach (var o in _circuit) Diagram.Add(o);
            _circuit = null;
        }
    }
}
