using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ECS.Converters;
using ECS.Model;
using ECS.Model.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace ECS.ViewModel
{
    public class DesignerViewModel : ViewModelBase
    {
        public DesignerViewModel()
        {
            _ser = new Serialization();
            DiagramObjects = new ObservableCollection<DiagramObject>();
            CursorMode = CursorMode.ArrangeItems;
            AreaHeight = 1000;
            AreaWidth = 1000;
            ColorConverter = new CurrentToColorConverter { MaxColor = Colors.Red, MinColor = Colors.Green };
        }

        private CursorMode _cursorMode;
        private DiagramObject _selectedObject;
        private int _nextResistorId;
        private int _nextVSourceId;
        private int _nextNodeId;
        private int _nextRefNodeId = -1;

        public DiagramObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                RaisePropertyChanged();
            }
        }

        [NotNull]
        public ObservableCollection<DiagramObject> DiagramObjects { get; }

        [NotNull]
        public IEnumerable<Node> Nodes => DiagramObjects.OfType<Node>();

        public bool AllowDrag => CursorMode == CursorMode.ArrangeItems;
        public double AreaHeight { get; set; }
        public double AreaWidth { get; set; }
        public CurrentToColorConverter ColorConverter { get; set; }

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

        public ICommand ClickCommand => new RelayCommand<Point>(OnClick);
        public ICommand LoadCommand => new RelayCommand(Load);
        public ICommand SaveCommand => new RelayCommand(Save);

        private void OnClick(Point e)
        {
            switch (CursorMode)
            {
                case CursorMode.AddResistor:
                    DiagramObjects.Add(new Resistor { Id = _nextResistorId++, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddVoltageSource:
                    DiagramObjects.Add(new VoltageSource { Id = _nextVSourceId++, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddNode:
                    DiagramObjects.Add(new Node { Id = _nextNodeId++, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddRefNode:
                    DiagramObjects.Add(new Node { Id = _nextRefNodeId--, X = e.X, Y = e.Y });
                    break;
            }
        }

        private readonly Serialization _ser;

        private OpenFileDialog _ofd;
        private void Load()
        {
            if (_ofd == null) _ofd = new OpenFileDialog { Filter = "XML file|*.xml" };
            var dr = _ofd.ShowDialog(Application.Current.MainWindow);
            if (dr != true) return;
            // TODO: Refactor this and update next node ids
            CircuitXml cx;
            using (var fs = File.OpenRead(_ofd.FileName))
                cx = _ser.Deserialize(fs);

            var nodes = cx.Nodes.Where(n => n != null).ToDictionary(n => n.Id);
            Node ln;
            foreach (var r in cx.Resistors.Where(r => r != null))
            {
                if ((r.Node1Id != null) && nodes.TryGetValue(r.Node1Id.Value, out ln)) r.Node1 = ln;
                if ((r.Node2Id != null) && nodes.TryGetValue(r.Node2Id.Value, out ln)) r.Node2 = ln;
            }
            foreach (var v in cx.VoltageSources)
            {
                if ((v.Node1Id != null) && nodes.TryGetValue(v.Node1Id.Value, out ln)) v.Node1 = ln;
                if ((v.Node2Id != null) && nodes.TryGetValue(v.Node2Id.Value, out ln)) v.Node2 = ln;
            }

            DiagramObjects.Clear();
            cx.Nodes.ForEach(n => DiagramObjects.Add(n));
            cx.Resistors.ForEach(n => DiagramObjects.Add(n));
            cx.VoltageSources.ForEach(n => DiagramObjects.Add(n));
            _nextResistorId = DiagramObjects.OfType<Resistor>().Count();
            _nextVSourceId = DiagramObjects.OfType<VoltageSource>().Count();
            _nextNodeId = DiagramObjects.OfType<Node>().Count(n => n?.Id > -1);
            _nextRefNodeId = -DiagramObjects.OfType<Node>().Count(n => n?.Id < 0) - 1;

            CursorMode = CursorMode.ArrangeItems;
        }

        private SaveFileDialog _sfd;
        private void Save()
        {
            if (_sfd == null) _sfd = new SaveFileDialog { Filter = "XML file|*.xml" };
            var dr = _sfd.ShowDialog(Application.Current.MainWindow);
            if (dr != true) return;

            var cx = new CircuitXml();
            cx.Nodes.AddRange(DiagramObjects.OfType<Node>());
            cx.Resistors.AddRange(DiagramObjects.OfType<Resistor>());
            cx.VoltageSources.AddRange(DiagramObjects.OfType<VoltageSource>());
            using (var fs = File.Create(_sfd.FileName))
                _ser.Serialize(cx, fs);
        }
    }

    public enum CursorMode
    {
        ArrangeItems,
        ConnectToNode,
        AddResistor,
        AddVoltageSource,
        AddNode,
        AddRefNode
    }
}
