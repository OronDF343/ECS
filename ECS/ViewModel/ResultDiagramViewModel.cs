using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ECS.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;

namespace ECS.ViewModel
{
    public class ResultDiagramViewModel : ViewModelBase
    {
        public ResultDiagramViewModel([NotNull] IEnumerable<DiagramObject> diagram, string name)
        {
            _diagram = diagram;
            Name = name;
        }

        private readonly IEnumerable<DiagramObject> _diagram;

        public ObservableCollection<DiagramObject> DiagramObjects { get; } = new ObservableCollection<DiagramObject>();
        public string Name { get; }

        public ICommand LoadedCommand => new RelayCommand(OnLoaded);

        private void OnLoaded()
        {
            foreach (var dobj in _diagram)
            {
                DiagramObjects.Add(dobj);
            }
        }
    }
}
