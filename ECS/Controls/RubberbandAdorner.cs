using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ECS.Controls
{
    public class RubberbandAdorner : Adorner
    {
        private Point? _startPoint, _endPoint;
        private readonly Rectangle _rubberband;
        private readonly DesignerCanvas _designerCanvas;
        private readonly VisualCollection _visuals;
        private readonly Canvas _adornerCanvas;

        protected override int VisualChildrenCount => _visuals.Count;

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this._designerCanvas = designerCanvas;
            _startPoint = dragStartPoint;

            _adornerCanvas = new Canvas { Background = Brushes.Transparent };
            _visuals = new VisualCollection(this) { _adornerCanvas };

            _rubberband = new Rectangle
            {
                Stroke = Brushes.Navy,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new[] { 2.0 })
            };

            _adornerCanvas.Children?.Add(_rubberband);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!IsMouseCaptured) CaptureMouse();

            _endPoint = e.GetPosition(this);
            UpdateRubberband();
            UpdateSelection();
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured) ReleaseMouseCapture();

            var adornerLayer = Parent as AdornerLayer;
            adornerLayer?.Remove(this);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _adornerCanvas?.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals?[index];
        }

        private void UpdateRubberband()
        {
            var left = Math.Min(_startPoint.Value.X, _endPoint.Value.X);
            var top = Math.Min(_startPoint.Value.Y, _endPoint.Value.Y);

            var width = Math.Abs(_startPoint.Value.X - _endPoint.Value.X);
            var height = Math.Abs(_startPoint.Value.Y - _endPoint.Value.Y);

            _rubberband.Width = width;
            _rubberband.Height = height;
            Canvas.SetLeft(_rubberband, left);
            Canvas.SetTop(_rubberband, top);
        }

        private void UpdateSelection()
        {
            var rubberBand = new Rect(_startPoint.Value, _endPoint.Value);
            foreach (DesignerItem item in _designerCanvas.Children)
            {
                var itemRect = VisualTreeHelper.GetDescendantBounds(item);
                var itemBounds = item.TransformToAncestor(_designerCanvas).TransformBounds(itemRect);

                item.IsSelected = rubberBand.Contains(itemBounds);
            }
        }
    }
}
