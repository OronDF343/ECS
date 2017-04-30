using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private CursorMode _cursorMode;
        private int _nextNodeId;
        private int _nextRefNodeId;
        private int _nextResistorId;
        private int _nextVSourceId;
        private int _nextSwitchId;

        private OpenFileDialog _ofd;
        private DiagramObject _selectedObject;

        private SaveFileDialog _sfd;
        private double _componentClickPos;
        private DiagramObject _lastSelectedObject;

        public DiagramObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _lastSelectedObject = _selectedObject;
                _selectedObject = value;
                RaisePropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(SelectedObject) && CursorMode == CursorMode.ConnectToNode)
            {
                var t = TryConnectToNode();
                base.RaisePropertyChanged(propertyName);
                if (t)
                {
                    _selectedObject = null;
                    base.RaisePropertyChanged(propertyName);
                }
            }
            else base.RaisePropertyChanged(propertyName);
        }

        private bool TryConnectToNode()
        {
            var n = _selectedObject as Node;
            var c = _lastSelectedObject as Component;
            if (n == null && c == null)
            {
                n = _lastSelectedObject as Node;
                c = _selectedObject as Component;
                // If we just selected the component, save the mouse X coordinate
                if (c != null)
                {
                    var di = (Application.Current.MainWindow as MainWindow)
                    .EditBox.Children.OfType<DesignerItem>().FirstOrDefault(d => Equals(d.DataContext, c));
                    if (di == null) return false;
                    _componentClickPos = Mouse.GetPosition(di).X;
                }
            }
            // By now n and c should contain correct objects
            if (n == null || c == null) return false;
            if (_componentClickPos > 26 && _componentClickPos < 48) c.Node2 = n;
            else if (_componentClickPos > 0 && _componentClickPos < 22) c.Node1 = n;
            return true;
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
            get { return _cursorMode; }
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

        public bool AreStatesEnabled { get; set; }
        public ObservableCollection<CircuitState> SimulationStates { get; set; } = new ObservableCollection<CircuitState>();

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
            if (AreStatesEnabled)
            {
                var diags = new ObservableCollection<TabItem>();
                var r = ViewMaker.CreateResults(diags);
                r.Owner = Application.Current.MainWindow;
                foreach (var state in SimulationStates)
                {
                    // Apply state
                    foreach (var switchState in state.SwitchStates)
                    {
                        Switches.FirstOrDefault(sw => sw.Id == switchState.Key).IsClosed = switchState.Value;
                    }
                    // Update simulation
                    var s = UpdateSimulation();
                    if (s == null) Application.Current.Dispatcher.Invoke(() => diags.Add(ViewMaker.CreateResultDiagramSnapshot(state.Name)), DispatcherPriority.Background);
                    else diags.Add(ViewMaker.CreateResultError(s));
                    // TODO: Clear results
                }
                r.ShowDialog();
            }
            else
            {
                var s = UpdateSimulation();
                if (s != null)
                    MessageBox.Show(Application.Current.MainWindow, s, "Simulation Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }

        private string UpdateSimulation()
        {
            try
            {
                Simulator.AnalyzeAndUpdate(Nodes, DiagramObjects.OfType<IComponent>());
                return null;
            }
            catch (Exception ex)
            {
                return "Simulation error: " + ex;
            }
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
