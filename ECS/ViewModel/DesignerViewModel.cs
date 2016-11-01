using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace ECS.ViewModel
{
    public class DesignerViewModel : ViewModelBase
    {
        private CursorMode _cursorMode;

        public DesignerViewModel()
        {
            Resistors = new ObservableCollection<Resistor>();
            VoltageSources = new ObservableCollection<VoltageSource>();
            Nodes = new ObservableCollection<Node>();
            CursorMode = CursorMode.ArrangeItems;
            AreaHeight = 400;
            AreaWidth = 400;
            var n = new Node();
            var n2 = new Node();
            var c = new Resistor { Node1 = n, Node2 = n2 };
            Resistors.Add(c);
            Nodes.Add(n);
            Nodes.Add(n2);
        }

        public DiagramObject SelectedObject { get; set; }
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
