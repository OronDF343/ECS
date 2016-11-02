using System.Collections.ObjectModel;
using ECS.Model;
using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class DesignerViewModel : ViewModelBase
    {
        public DesignerViewModel()
        {
            Resistors = new ObservableCollection<Resistor>();
            VoltageSources = new ObservableCollection<VoltageSource>();
            Nodes = new ObservableCollection<Node>();
            CursorMode = CursorMode.ArrangeItems;
            AreaHeight = 1000;
            AreaWidth = 1000;
            var n = new Node { Id = -1 };
            var n2 = new Node { Id = 0 };
            var c = new Resistor { Id = 0, Node1 = n, Node2 = n2 };
            Resistors.Add(c);
            Nodes.Add(n);
            Nodes.Add(n2);
        }

        private CursorMode _cursorMode;
        private DiagramObject _selectedObject;

        public DiagramObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Resistor> Resistors { get; }
        public ObservableCollection<VoltageSource> VoltageSources { get; }
        public ObservableCollection<Node> Nodes { get; }
        public bool AllowDrag => CursorMode == CursorMode.ArrangeItems;
        public double AreaHeight { get; set; }
        public double AreaWidth { get; set; }

        public CursorMode CursorMode
        {
            get { return _cursorMode; }
            set
            {
                _cursorMode = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AllowDrag));
            }
        }
    }

    public enum CursorMode
    {
        ArrangeItems,
        ConnectToNode,
        AddResistor,
        AddVoltageSource
    }
}
