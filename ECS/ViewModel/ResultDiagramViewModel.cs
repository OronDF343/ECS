using System.Collections.Generic;
using System.Collections.ObjectModel;
using ECS.Model;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace ECS.ViewModel
{
    public class ResultDiagramViewModel : ViewModelBase
    {
        public ResultDiagramViewModel([NotNull] IEnumerable<DiagramObject> diagram, string name)
        {
            DiagramObjects = new ObservableCollection<DiagramObject>(diagram);
            Name = name;
        }

        public ObservableCollection<DiagramObject> DiagramObjects { get; }
        public string Name { get; }
    }
}
