using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ECS.Model;
using ECS.Model.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace ECS.ViewModel
{
    public class DesignerViewModel : ViewModelBase
    {
        public DesignerViewModel()
        {
            _ser = new Serialization();
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
        private int _nextResistorId;
        private int _nextVSourceId;
        // TODO: Next node and reference node id!

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
        public ObservableCollection<Resistor> Resistors { get; }
        [NotNull]
        public ObservableCollection<VoltageSource> VoltageSources { get; }
        [NotNull]
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

        public ICommand ClickCommand => new RelayCommand<Point>(OnClick);
        public ICommand LoadCommand => new RelayCommand(Load);
        public ICommand SaveCommand => new RelayCommand(Save);

        private void OnClick(Point e)
        {
            if (CursorMode == CursorMode.AddResistor) Resistors.Add(new Resistor { Id = _nextResistorId++, X = e.X, Y = e.Y });
            if (CursorMode == CursorMode.AddVoltageSource) VoltageSources.Add(new VoltageSource { Id = _nextVSourceId++, X = e.X, Y = e.Y });
        }

        private Serialization _ser;

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

            Nodes.Clear();
            Resistors.Clear();
            VoltageSources.Clear();
            cx.Nodes.ForEach(n => Nodes.Add(n));
            cx.Resistors.ForEach(n => Resistors.Add(n));
            cx.VoltageSources.ForEach(n => VoltageSources.Add(n));
            _nextResistorId = Resistors.Count;
            _nextVSourceId = VoltageSources.Count;

            CursorMode = CursorMode.ArrangeItems;
        }

        private SaveFileDialog _sfd;
        private void Save()
        {
            if (_sfd == null) _sfd = new SaveFileDialog { Filter = "XML file|*.xml" };
            var dr = _sfd.ShowDialog(Application.Current.MainWindow);
            if (dr != true) return;

            var cx = new CircuitXml();
            cx.Nodes.AddRange(Nodes);
            cx.Resistors.AddRange(Resistors);
            cx.VoltageSources.AddRange(VoltageSources);
            using (var fs = File.Create(_sfd.FileName))
                _ser.Serialize(cx, fs);
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
