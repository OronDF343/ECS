﻿using GalaSoft.MvvmLight;
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
            var n2 = new Node();
            var c = new Component { Node1 = n, Node2 = n2 };
            Components.Add(c);
            Nodes.Add(n);
            Nodes.Add(n2);
        }

        public DiagramObject SelectedObject { get; set; }
        public ObservableCollection<Component> Components { get; }
        public ObservableCollection<Node> Nodes { get; }
        public bool AllowDrag { get; } = true;
        public double AreaHeight { get; set; }
        public double AreaWidth { get; set; }
    }
}
