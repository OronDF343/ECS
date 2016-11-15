using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Controls
{
    public class Connection : Control, ISelectable, INotifyPropertyChanged
    {
        public Connection(Connector source, Connector sink)
        {
            Id = Guid.NewGuid();
            Source = source;
            Sink = sink;
            Unloaded += Connection_Unloaded;
        }

        private Adorner _connectionAdorner;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // usual selection business
            var designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (IsSelected) designer.SelectionService.RemoveFromSelection(this);
                    else designer.SelectionService.AddToSelection(this);
                else if (!IsSelected) designer.SelectionService.SelectItem(this);

                Focus();
            }
            e.Handled = false;
        }

        private void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            // whenever the 'Position' property of the source or sink Connector 
            // changes we must update the connection path geometry
            if (e.PropertyName.Equals("Position")) UpdatePathGeometry();
        }

        private void UpdatePathGeometry()
        {
            if ((Source == null) || (Sink == null)) return;
            var geometry = new PathGeometry();
            var linePoints = PathFinder.GetConnectionLine(Source.GetInfo(), Sink.GetInfo(), true);
            if (linePoints.Count <= 0) return;
            var figure = new PathFigure { StartPoint = linePoints[0] };
            linePoints.Remove(linePoints[0]);
            figure.Segments.Add(new PolyLineSegment(linePoints, true));
            geometry.Figures.Add(figure);

            PathGeometry = geometry;
        }

        private void UpdateAnchorPosition()
        {
            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;
            Point pathMidPoint, pathTangentAtMidPoint;

            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
            // on PathGeometry at the specified fraction of its length
            PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
            PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
            PathGeometry.GetPointAtFractionLength(0.5, out pathMidPoint, out pathTangentAtMidPoint);

            // get angle from tangent vector
            AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            // add some margin on source and sink side for visual reasons only
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            AnchorPositionSource = pathStartPoint;
            AnchorPositionSink = pathEndPoint;
            LabelPosition = pathMidPoint;
        }

        private void ShowAdorner()
        {
            // the ConnectionAdorner is created once for each Connection
            if (_connectionAdorner == null)
            {
                var designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    _connectionAdorner = new ConnectionAdorner(designer, this);
                    adornerLayer.Add(_connectionAdorner);
                }
            }
            _connectionAdorner.Visibility = Visibility.Visible;
        }

        private void HideAdorner()
        {
            if (_connectionAdorner != null) _connectionAdorner.Visibility = Visibility.Collapsed;
        }

        private void Connection_Unloaded(object sender, RoutedEventArgs e)
        {
            // do some housekeeping when Connection is unloaded

            // remove event handler
            Source = null;
            Sink = null;

            // remove adorner
            if (_connectionAdorner == null) return;
            //var designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer == null) return;
            adornerLayer.Remove(_connectionAdorner);
            _connectionAdorner = null;
        }

        #region Properties

        public Guid Id { get; set; }

        // source connector
        private Connector _source;

        public Connector Source
        {
            get { return _source; }
            set
            {
                if (Equals(_source, value)) return;
                if (_source != null)
                {
                    _source.PropertyChanged -= OnConnectorPositionChanged;
                    _source.Connections.Remove(this);
                }

                _source = value;

                if (_source != null)
                {
                    _source.Connections.Add(this);
                    _source.PropertyChanged += OnConnectorPositionChanged;
                }

                UpdatePathGeometry();
            }
        }

        // sink connector
        private Connector _sink;

        public Connector Sink
        {
            get { return _sink; }
            set
            {
                if (Equals(_sink, value)) return;
                if (_sink != null)
                {
                    _sink.PropertyChanged -= OnConnectorPositionChanged;
                    _sink.Connections.Remove(this);
                }

                _sink = value;

                if (_sink != null)
                {
                    _sink.Connections.Add(this);
                    _sink.PropertyChanged += OnConnectorPositionChanged;
                }
                UpdatePathGeometry();
            }
        }

        // connection path geometry
        private PathGeometry _pathGeometry;

        public PathGeometry PathGeometry
        {
            get { return _pathGeometry; }
            set
            {
                if (Equals(_pathGeometry, value)) return;
                _pathGeometry = value;
                UpdateAnchorPosition();
                OnPropertyChanged("PathGeometry");
            }
        }

        // between source connector position and the beginning 
        // of the path geometry we leave some space for visual reasons; 
        // so the anchor position source really marks the beginning 
        // of the path geometry on the source side
        private Point _anchorPositionSource;

        public Point AnchorPositionSource
        {
            get { return _anchorPositionSource; }
            set
            {
                if (_anchorPositionSource == value) return;
                _anchorPositionSource = value;
                OnPropertyChanged("AnchorPositionSource");
            }
        }

        // slope of the path at the anchor position
        // needed for the rotation angle of the arrow
        private double _anchorAngleSource;

        public double AnchorAngleSource
        {
            get { return _anchorAngleSource; }
            set
            {
                if (_anchorAngleSource == value) return;
                _anchorAngleSource = value;
                OnPropertyChanged("AnchorAngleSource");
            }
        }

        // analogue to source side
        private Point _anchorPositionSink;

        public Point AnchorPositionSink
        {
            get { return _anchorPositionSink; }
            set
            {
                if (_anchorPositionSink == value) return;
                _anchorPositionSink = value;
                OnPropertyChanged("AnchorPositionSink");
            }
        }

        // analogue to source side
        private double _anchorAngleSink;

        public double AnchorAngleSink
        {
            get { return _anchorAngleSink; }
            set
            {
                if (_anchorAngleSink == value) return;
                _anchorAngleSink = value;
                OnPropertyChanged("AnchorAngleSink");
            }
        }

        private ArrowSymbol _sourceArrowSymbol = ArrowSymbol.None;

        public ArrowSymbol SourceArrowSymbol
        {
            get { return _sourceArrowSymbol; }
            set
            {
                if (_sourceArrowSymbol == value) return;
                _sourceArrowSymbol = value;
                OnPropertyChanged("SourceArrowSymbol");
            }
        }

        public ArrowSymbol sinkArrowSymbol = ArrowSymbol.Arrow;

        public ArrowSymbol SinkArrowSymbol
        {
            get { return sinkArrowSymbol; }
            set
            {
                if (sinkArrowSymbol == value) return;
                sinkArrowSymbol = value;
                OnPropertyChanged("SinkArrowSymbol");
            }
        }

        // specifies a point at half path length
        private Point _labelPosition;

        public Point LabelPosition
        {
            get { return _labelPosition; }
            set
            {
                if (_labelPosition == value) return;
                _labelPosition = value;
                OnPropertyChanged("LabelPosition");
            }
        }

        // pattern of dashes and gaps that is used to outline the connection path
        private DoubleCollection _strokeDashArray;

        public DoubleCollection StrokeDashArray
        {
            get { return _strokeDashArray; }
            set
            {
                if (Equals(_strokeDashArray, value)) return;
                _strokeDashArray = value;
                OnPropertyChanged("StrokeDashArray");
            }
        }

        // if connected, the ConnectionAdorner becomes visible
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
                if (_isSelected) ShowAdorner();
                else HideAdorner();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    public enum ArrowSymbol
    {
        None,
        Arrow,
        Diamond
    }
}
