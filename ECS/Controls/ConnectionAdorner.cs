using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Controls
{
    public class ConnectionAdorner : Adorner
    {
        private readonly Canvas _adornerCanvas;
        private readonly Connection _connection;
        private readonly DesignerCanvas _designerCanvas;
        private readonly Pen _drawingPen;
        private Connector _fixConnector, _dragConnector;

        private Connector _hitConnector;

        private DesignerItem _hitDesignerItem;
        private PathGeometry _pathGeometry;
        private Thumb _sourceDragThumb, _sinkDragThumb;

        private readonly VisualCollection _visualChildren;

        public ConnectionAdorner(DesignerCanvas designer, Connection connection)
            : base(designer)
        {
            _designerCanvas = designer;
            _adornerCanvas = new Canvas();
            _visualChildren = new VisualCollection(this);
            _visualChildren.Add(_adornerCanvas);

            this._connection = connection;
            this._connection.PropertyChanged += AnchorPositionChanged;

            InitializeDragThumbs();

            _drawingPen = new Pen(Brushes.LightSlateGray, 1);
            _drawingPen.LineJoin = PenLineJoin.Round;

            Unloaded += ConnectionAdorner_Unloaded;
        }

        private DesignerItem HitDesignerItem
        {
            get { return _hitDesignerItem; }
            set
            {
                if (_hitDesignerItem != value)
                {
                    if (_hitDesignerItem != null)
                        _hitDesignerItem.IsDragConnectionOver = false;

                    _hitDesignerItem = value;

                    if (_hitDesignerItem != null)
                        _hitDesignerItem.IsDragConnectionOver = true;
                }
            }
        }

        private Connector HitConnector
        {
            get { return _hitConnector; }
            set
            {
                if (_hitConnector != value)
                    _hitConnector = value;
            }
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChildren[index];
        }


        private void AnchorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("AnchorPositionSource"))
            {
                Canvas.SetLeft(_sourceDragThumb, _connection.AnchorPositionSource.X);
                Canvas.SetTop(_sourceDragThumb, _connection.AnchorPositionSource.Y);
            }

            if (e.PropertyName.Equals("AnchorPositionSink"))
            {
                Canvas.SetLeft(_sinkDragThumb, _connection.AnchorPositionSink.X);
                Canvas.SetTop(_sinkDragThumb, _connection.AnchorPositionSink.Y);
            }
        }

        private void thumbDragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (HitConnector != null)
                if (_connection != null)
                    if (_connection.Source == _fixConnector)
                        _connection.Sink = HitConnector;
                    else
                        _connection.Source = HitConnector;

            HitDesignerItem = null;
            HitConnector = null;
            _pathGeometry = null;
            _connection.StrokeDashArray = null;
            InvalidateVisual();
        }

        private void thumbDragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            HitDesignerItem = null;
            HitConnector = null;
            _pathGeometry = null;
            Cursor = Cursors.Cross;
            _connection.StrokeDashArray = new DoubleCollection(new double[] {1, 2});

            if (sender == _sourceDragThumb)
            {
                _fixConnector = _connection.Sink;
                _dragConnector = _connection.Source;
            }
            else if (sender == _sinkDragThumb)
            {
                _dragConnector = _connection.Sink;
                _fixConnector = _connection.Source;
            }
        }

        private void thumbDragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var currentPosition = Mouse.GetPosition(this);
            HitTesting(currentPosition);
            _pathGeometry = UpdatePathGeometry(currentPosition);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, _drawingPen, _pathGeometry);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerCanvas.Arrange(new Rect(0, 0, _designerCanvas.ActualWidth, _designerCanvas.ActualHeight));
            return finalSize;
        }

        private void ConnectionAdorner_Unloaded(object sender, RoutedEventArgs e)
        {
            _sourceDragThumb.DragDelta -= thumbDragThumb_DragDelta;
            _sourceDragThumb.DragStarted -= thumbDragThumb_DragStarted;
            _sourceDragThumb.DragCompleted -= thumbDragThumb_DragCompleted;

            _sinkDragThumb.DragDelta -= thumbDragThumb_DragDelta;
            _sinkDragThumb.DragStarted -= thumbDragThumb_DragStarted;
            _sinkDragThumb.DragCompleted -= thumbDragThumb_DragCompleted;
        }

        private void InitializeDragThumbs()
        {
            var dragThumbStyle = _connection.FindResource("ConnectionAdornerThumbStyle") as Style;

            //source drag thumb
            _sourceDragThumb = new Thumb();
            Canvas.SetLeft(_sourceDragThumb, _connection.AnchorPositionSource.X);
            Canvas.SetTop(_sourceDragThumb, _connection.AnchorPositionSource.Y);
            _adornerCanvas.Children.Add(_sourceDragThumb);
            if (dragThumbStyle != null)
                _sourceDragThumb.Style = dragThumbStyle;

            _sourceDragThumb.DragDelta += thumbDragThumb_DragDelta;
            _sourceDragThumb.DragStarted += thumbDragThumb_DragStarted;
            _sourceDragThumb.DragCompleted += thumbDragThumb_DragCompleted;

            // sink drag thumb
            _sinkDragThumb = new Thumb();
            Canvas.SetLeft(_sinkDragThumb, _connection.AnchorPositionSink.X);
            Canvas.SetTop(_sinkDragThumb, _connection.AnchorPositionSink.Y);
            _adornerCanvas.Children.Add(_sinkDragThumb);
            if (dragThumbStyle != null)
                _sinkDragThumb.Style = dragThumbStyle;

            _sinkDragThumb.DragDelta += thumbDragThumb_DragDelta;
            _sinkDragThumb.DragStarted += thumbDragThumb_DragStarted;
            _sinkDragThumb.DragCompleted += thumbDragThumb_DragCompleted;
        }

        private PathGeometry UpdatePathGeometry(Point position)
        {
            var geometry = new PathGeometry();

            ConnectorOrientation targetOrientation;
            if (HitConnector != null)
                targetOrientation = HitConnector.Orientation;
            else
                targetOrientation = _dragConnector.Orientation;

            var linePoints = PathFinder.GetConnectionLine(_fixConnector.GetInfo(), position, targetOrientation);

            if (linePoints.Count > 0)
            {
                var figure = new PathFigure();
                figure.StartPoint = linePoints[0];
                linePoints.Remove(linePoints[0]);
                figure.Segments.Add(new PolyLineSegment(linePoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        private void HitTesting(Point hitPoint)
        {
            var hitConnectorFlag = false;

            var hitObject = _designerCanvas.InputHitTest(hitPoint) as DependencyObject;
            while ((hitObject != null) &&
                   (hitObject != _fixConnector.ParentDesignerItem) &&
                   (hitObject.GetType() != typeof(DesignerCanvas)))
            {
                if (hitObject is Connector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                }

                if (hitObject is DesignerItem)
                {
                    HitDesignerItem = hitObject as DesignerItem;
                    if (!hitConnectorFlag)
                        HitConnector = null;
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitDesignerItem = null;
        }
    }
}