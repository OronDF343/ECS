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
        private Point? startPoint, endPoint;
        private Rectangle rubberband;
        private DesignerCanvas designerCanvas;
        private VisualCollection visuals;
        private Canvas adornerCanvas;

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            startPoint = dragStartPoint;

            adornerCanvas = new Canvas();
            adornerCanvas.Background = Brushes.Transparent;
            visuals = new VisualCollection(this);
            visuals.Add(adornerCanvas);

            rubberband = new Rectangle();
            rubberband.Stroke = Brushes.Navy;
            rubberband.StrokeThickness = 1;
            rubberband.StrokeDashArray = new DoubleCollection(new double[] { 2 });

            adornerCanvas.Children.Add(rubberband);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                endPoint = e.GetPosition(this);
                UpdateRubberband();
                UpdateSelection();
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            var adornerLayer = Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            adornerCanvas.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        private void UpdateRubberband()
        {
            var left = Math.Min(startPoint.Value.X, endPoint.Value.X);
            var top = Math.Min(startPoint.Value.Y, endPoint.Value.Y);

            var width = Math.Abs(startPoint.Value.X - endPoint.Value.X);
            var height = Math.Abs(startPoint.Value.Y - endPoint.Value.Y);

            rubberband.Width = width;
            rubberband.Height = height;
            Canvas.SetLeft(rubberband, left);
            Canvas.SetTop(rubberband, top);
        }

        private void UpdateSelection()
        {
            var rubberBand = new Rect(startPoint.Value, endPoint.Value);
            foreach (DesignerItem item in designerCanvas.Children)
            {
                var itemRect = VisualTreeHelper.GetDescendantBounds(item);
                var itemBounds = item.TransformToAncestor(designerCanvas).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }
    }
}
