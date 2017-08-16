using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ECS.Layout
{
    public class DragThumb : Thumb
    {
        public DragThumb()
        {
            DragDelta += DragThumb_DragDelta;
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as DesignerItem;
            var designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
            if (designer == null || designer.SelectedItem != designerItem.DataContext) return;

            // Get current position of item
            var left = Canvas.GetLeft(designerItem);
            var top = Canvas.GetTop(designerItem);
            left = double.IsNaN(left) ? 0 : left;
            top = double.IsNaN(top) ? 0 : top;

            // Prevent out-of-bounds at top/left
            var deltaHorizontal = Math.Max(-left, e.HorizontalChange);
            var deltaVertical = Math.Max(-top, e.VerticalChange);

            // Prevent out-of-bounds at bottom/right, and make sure the item will stay visible
            deltaHorizontal = Math.Min(deltaHorizontal, designer.ActualWidth - left - 24);
            deltaVertical = Math.Min(deltaVertical, designer.ActualHeight - top - 24);

            // Update position
            Canvas.SetLeft(designerItem, left + deltaHorizontal);
            Canvas.SetTop(designerItem, top + deltaVertical);

            designer.InvalidateMeasure();
            e.Handled = true;
        }
    }
}
