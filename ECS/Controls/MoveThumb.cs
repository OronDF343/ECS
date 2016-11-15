using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ECS.Controls
{
    class MoveThumb : Thumb
    {
        private DesignerItem designerItem;
        private DesignerCanvas designerCanvas;

        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            designerItem = DataContext as DesignerItem;

            if (designerItem != null)
            {
                designerCanvas = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (designerItem != null && designerCanvas != null && designerItem.IsSelected)
            {
                var minLeft = double.MaxValue;
                var minTop = double.MaxValue;

                foreach (var item in designerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                }

                var deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                var deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (var item in designerCanvas.SelectedItems)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                }

                designerCanvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
