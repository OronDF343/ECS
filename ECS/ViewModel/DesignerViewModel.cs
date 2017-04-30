using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ECS.Core;
using ECS.Core.Model;
using ECS.Layout;
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
            DiagramObjects = new ObservableCollection<DiagramObject>();
            CursorMode = CursorMode.ArrangeItems;
            AreaHeight = 1000;
            AreaWidth = 1000;
        }

        private readonly Serialization _ser;
        private double _componentClickPos;

        private CursorMode _cursorMode;
        private int _nextNodeId;
        private int _nextRefNodeId;
        private int _nextResistorId;
        private int _nextSwitchId;
        private int _nextVSourceId;

        private OpenFileDialog _ofd;
        private DiagramObject _selectedObject;
        private SaveFileDialog _sfd;

        public DiagramObject SelectedObject
        {
            get => _selectedObject;
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

        public IEnumerable<Switch> Switches => DiagramObjects.OfType<Switch>();

        public bool AllowDrag => CursorMode == CursorMode.ArrangeItems;
        public double AreaHeight { get; set; }
        public double AreaWidth { get; set; }

        public CursorMode CursorMode
        {
            get => _cursorMode;
            set
            {
                _cursorMode = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AllowDrag));
            }
        }

        public ICommand ClickCommand => new RelayCommand<Point>(OnClick);
        public ICommand DeleteCommand => new RelayCommand(Delete);
        public ICommand LoadCommand => new RelayCommand(Load);
        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand SimulateCommand => new RelayCommand(Simulate);
        public ICommand StatesEditorCommand => new RelayCommand(OpenStatesEditor);

        public ICommand SelectedItemChangedCommand =>
            new RelayCommand<SelectedItemChangedEventArgs>(SelectedItemChanged);

        public bool AreStatesEnabled { get; set; }

        public ObservableCollection<CircuitState> SimulationStates { get; set; } =
            new ObservableCollection<CircuitState>();

        private void SelectedItemChanged(SelectedItemChangedEventArgs obj)
        {
            if (CursorMode != CursorMode.ConnectToNode) return;
            var n = obj.NewValue as Node;
            var c = obj.OldValue as Component;
            if (n == null && c == null)
            {
                n = obj.OldValue as Node;
                c = obj.NewValue as Component;
                // If we just selected the component, save the mouse X coordinate
                if (c != null)
                {
                    var di = (Application.Current.MainWindow as MainWindow)
                        .EditBox.Children.OfType<DesignerItem>().FirstOrDefault(d => Equals(d.DataContext, c));
                    if (di == null) return;
                    _componentClickPos = Mouse.GetPosition(di).X;
                }
            }
            // By now n and c should contain correct objects
            if (n == null || c == null) return;
            if (_componentClickPos > 26 && _componentClickPos < 48) c.Node2 = n;
            else if (_componentClickPos > 0 && _componentClickPos < 22) c.Node1 = n;
            SelectedObject = null;
        }

        private void OnClick(Point e)
        {
            switch (CursorMode)
            {
                case CursorMode.AddResistor:
                    DiagramObjects.Add(new Resistor { Name = @"R" + ++_nextResistorId, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddVoltageSource:
                    DiagramObjects.Add(new VoltageSource { Name = @"Vin" + ++_nextVSourceId, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddNode:
                    DiagramObjects.Add(new Node { Name = @"N" + ++_nextNodeId, X = e.X, Y = e.Y });
                    break;
                case CursorMode.AddRefNode:
                    DiagramObjects.Add(new Node
                    {
                        Name = @"Nref" + ++_nextRefNodeId,
                        X = e.X,
                        Y = e.Y,
                        IsReferenceNode = true
                    });
                    break;
                case CursorMode.AddSwitch:
                    DiagramObjects.Add(new Switch
                    {
                        Name = @"S" + ++_nextSwitchId,
                        X = e.X,
                        Y = e.Y
                    });
                    break;
            }
        }

        private void Delete()
        {
            if (SelectedObject != null) DiagramObjects.Remove(SelectedObject);
        }

        private void Load()
        {
            if (_ofd == null) _ofd = new OpenFileDialog { Filter = "XML file|*.xml" };
            var dr = _ofd.ShowDialog(Application.Current.MainWindow);
            if (dr != true) return;
            // TODO: update next node ids correctly?
            CircuitXml cx;
            using (var fs = File.OpenRead(_ofd.FileName)) { cx = _ser.Deserialize(fs); }
            DiagramObjects.Clear();
            foreach (var dobj in cx.ToDiagram()) DiagramObjects.Add(dobj);

            _nextResistorId = DiagramObjects.OfType<Resistor>().Count();
            _nextVSourceId = DiagramObjects.OfType<VoltageSource>().Count();
            _nextNodeId = DiagramObjects.OfType<Node>().Count(n => !n.IsReferenceNode);
            _nextRefNodeId = DiagramObjects.OfType<Node>().Count(n => n.IsReferenceNode);

            CursorMode = CursorMode.ArrangeItems;
        }

        private void Save()
        {
            if (_sfd == null) _sfd = new SaveFileDialog { Filter = "XML file|*.xml" };
            var dr = _sfd.ShowDialog(Application.Current.MainWindow);
            if (dr != true) return;

            var cx = CircuitXmlUtils.ToCircuitXml(DiagramObjects);
            using (var fs = File.Create(_sfd.FileName)) { _ser.Serialize(cx, fs); }
        }

        private void Simulate()
        {
            if (DiagramObjects.All(d => (d as Node)?.IsReferenceNode != true))
            {
                var mbr = MessageBox.Show(Application.Current.MainWindow,
                                          "Missing a reference node! Would you like an existing node to be automatically chosen as a reference node?",
                                          "Simulation warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mbr != MessageBoxResult.Yes) return;
                var vs = DiagramObjects.OfType<VoltageSource>().FirstOrDefault(v => v.Node2 != null);
                if (vs != null) vs.Node2.IsReferenceNode = true;
            }

            //
                var diags = new ObservableCollection<TabItem>();
                var r = ViewMaker.CreateResults(diags);
                r.Owner = Application.Current.MainWindow;
            var xml = _ser.Serialize(CircuitXmlUtils.ToCircuitXml(DiagramObjects));
            if (AreStatesEnabled)
            {
                foreach (var state in SimulationStates)
                {
                    var cir = _ser.Deserialize(xml).ToDiagram().ToList();
                    // Apply state
                    foreach (var switchState in state.SwitchStates)
                        cir.OfType<Switch>().FirstOrDefault(sw => sw.Id == switchState.Key).IsClosed =
                            switchState.Value;
                    // Update simulation
                    var s = UpdateSimulation(cir);
                    if (s == null) diags.Add(ViewMaker.CreateResultDiagramSnapshot(cir, state.Name));
                    else diags.Add(ViewMaker.CreateResultError(s));
                }
            }
            else
            {
                var cir = _ser.Deserialize(xml).ToDiagram().ToList();
                var s = UpdateSimulation(cir);
                if (s == null)
                    diags.Add(ViewMaker.CreateResultDiagramSnapshot(cir, "default"));
                else diags.Add(ViewMaker.CreateResultError(s));
            }
            r.ShowDialog();
        }

        private string UpdateSimulation(List<DiagramObject> cir)
        {
            try
            {
                Simulator.AnalyzeAndUpdate(cir.OfType<Node>(), cir.OfType<IComponent>());
                return null;
            }
            catch (Exception ex) { return "Simulation error: " + ex; }
        }

        private void OpenStatesEditor()
        {
            var sev = ViewMaker.CreateStatesEditor(SimulationStates, Switches);
            sev.Owner = Application.Current.MainWindow;
            sev.ShowDialog();
        }
    }

    public enum CursorMode
    {
        ArrangeItems,
        ConnectToNode,
        AddResistor,
        AddVoltageSource,
        AddNode,
        AddRefNode,
        AddSwitch
    }
}
