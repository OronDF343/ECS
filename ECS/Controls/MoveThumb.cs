using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ECS.Controls
{
    public class MoveThumb : Thumb
    {
        private DesignerItem _designerItem;
        private DesignerCanvas _designerCanvas;

        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _designerItem = DataContext as DesignerItem;

            if (_designerItem != null) _designerCanvas = VisualTreeHelper.GetParent(_designerItem) as DesignerCanvas;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_designerItem == null || _designerCanvas == null || !_designerItem.IsSelected) return;
            var minLeft = double.MaxValue;
            var minTop = double.MaxValue;

            foreach (var item in _designerCanvas.SelectedItems)
            {
                minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                minTop = Math.Min(Canvas.GetTop(item), minTop);
            }

            var deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
            var deltaVertical = Math.Max(-minTop, e.VerticalChange);

            foreach (var item in _designerCanvas.SelectedItems)
            {
                Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
            }

            _designerCanvas.InvalidateMeasure();
            e.Handled = true;
        }
    }
}
