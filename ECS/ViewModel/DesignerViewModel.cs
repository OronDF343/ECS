using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace ECS.ViewModel
{
    public class DesignerViewModel : ViewModelBase
    {
        public DesignerViewModel()
        {
            Components = new ObservableCollection<Component>();
            Nodes = new ObservableCollection<Node>();
            AreaHeight = 400;
            AreaWidth = 400;
            var n = new Node();
            var c = new Component();
            n.ComponentLinks.Add(new ComponentLink {Component = c, Node = n});
            c.Node1 = n;
            Components.Add(c);
            Nodes.Add(n);
        }

        public DiagramObject SelectedObject { get; set; }
        public ObservableCollection<Component> Components { get; }
        public ObservableCollection<Node> Nodes { get; }
        public bool AllowDrag { get; } = true;
        public double AreaHeight { get; set; }
        public double AreaWidth { get; set; }
    }
}
