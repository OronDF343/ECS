using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Controls
{
    public class RubberbandAdorner : Adorner
    {
        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            _designerCanvas = designerCanvas;
            _startPoint = dragStartPoint;
            _rubberbandPen = new Pen(Brushes.LightSlateGray, 1) { DashStyle = new DashStyle(new double[] { 2 }, 1) };
        }

        private readonly DesignerCanvas _designerCanvas;
        private readonly Pen _rubberbandPen;
        private Point? _endPoint;
        private Point? _startPoint;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured) CaptureMouse();

                _endPoint = e.GetPosition(this);
                UpdateSelection();
                InvalidateVisual();
            }
            else
            {
                if (IsMouseCaptured) ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            // release mouse capture
            if (IsMouseCaptured) ReleaseMouseCapture();

            // remove this adorner from adorner layer
            var adornerLayer = AdornerLayer.GetAdornerLayer(_designerCanvas);
            adornerLayer?.Remove(this);

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired!
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (_startPoint.HasValue && _endPoint.HasValue) dc.DrawRectangle(Brushes.Transparent, _rubberbandPen, new Rect(_startPoint.Value, _endPoint.Value));
        }

        private void UpdateSelection()
        {
            _designerCanvas.SelectionService.ClearSelection();

            var rubberBand = new Rect(_startPoint.Value, _endPoint.Value);
            foreach (Control item in _designerCanvas.Children)
            {
                var itemRect = VisualTreeHelper.GetDescendantBounds(item);
                var itemBounds = item.TransformToAncestor(_designerCanvas).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds))
                    if (item is Connection) _designerCanvas.SelectionService.AddToSelection(item as ISelectable);
                    else
                    {
                        var di = item as DesignerItem;
                        if (di.ParentId == Guid.Empty) _designerCanvas.SelectionService.AddToSelection(di);
                    }
            }
        }
    }
}
