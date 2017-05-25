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

            var left = Canvas.GetLeft(designerItem);
            var top = Canvas.GetTop(designerItem);
            left = double.IsNaN(left) ? 0 : left;
            top = double.IsNaN(top) ? 0 : top;

            var deltaHorizontal = Math.Max(-left, e.HorizontalChange);
            var deltaVertical = Math.Max(-top, e.VerticalChange);

            Canvas.SetLeft(designerItem, left + deltaHorizontal);
            Canvas.SetTop(designerItem, top + deltaVertical);

            designer.InvalidateMeasure();
            e.Handled = true;
        }
    }
}
