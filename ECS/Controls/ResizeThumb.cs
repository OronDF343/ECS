using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ECS.Controls
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as DesignerItem;
            var designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;

            if (designerItem == null || designer == null || !designerItem.IsSelected) return;
            double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
            double dragDeltaVertical, dragDeltaHorizontal, scale;

            var selectedDesignerItems = designer.SelectionService.CurrentSelection.OfType<DesignerItem>();

            CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop,
                                out minDeltaHorizontal, out minDeltaVertical);

            foreach (var item in selectedDesignerItems)
            {
                if (item == null || item.ParentId != Guid.Empty) continue;
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                        scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                        DragBottom(scale, item, designer.SelectionService);
                        break;
                    case VerticalAlignment.Top:
                        var top = Canvas.GetTop(item);
                        dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                        scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                        DragTop(scale, item, designer.SelectionService);
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        var left = Canvas.GetLeft(item);
                        dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                        scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                        DragLeft(scale, item, designer.SelectionService);
                        break;
                    case HorizontalAlignment.Right:
                        dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                        scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                        DragRight(scale, item, designer.SelectionService);
                        break;
                }
            }
            e.Handled = true;
        }

        #region Helper methods

        private static void DragLeft(double scale, DesignerItem item, SelectionService selectionService)
        {
            var groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();

            var groupLeft = Canvas.GetLeft(item) + item.Width;
            foreach (var groupItem in groupItems)
            {
                var groupItemLeft = Canvas.GetLeft(groupItem);
                var delta = (groupLeft - groupItemLeft) * (scale - 1);
                Canvas.SetLeft(groupItem, groupItemLeft - delta);
                groupItem.Width = groupItem.ActualWidth * scale;
            }
        }

        private static void DragTop(double scale, DesignerItem item, SelectionService selectionService)
        {
            var groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();
            var groupBottom = Canvas.GetTop(item) + item.Height;
            foreach (var groupItem in groupItems)
            {
                var groupItemTop = Canvas.GetTop(groupItem);
                var delta = (groupBottom - groupItemTop) * (scale - 1);
                Canvas.SetTop(groupItem, groupItemTop - delta);
                groupItem.Height = groupItem.ActualHeight * scale;
            }
        }

        private static void DragRight(double scale, DesignerItem item, SelectionService selectionService)
        {
            var groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();

            var groupLeft = Canvas.GetLeft(item);
            foreach (var groupItem in groupItems)
            {
                var groupItemLeft = Canvas.GetLeft(groupItem);
                var delta = (groupItemLeft - groupLeft) * (scale - 1);

                Canvas.SetLeft(groupItem, groupItemLeft + delta);
                groupItem.Width = groupItem.ActualWidth * scale;
            }
        }

        private static void DragBottom(double scale, DesignerItem item, SelectionService selectionService)
        {
            var groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();
            var groupTop = Canvas.GetTop(item);
            foreach (var groupItem in groupItems)
            {
                var groupItemTop = Canvas.GetTop(groupItem);
                var delta = (groupItemTop - groupTop) * (scale - 1);

                Canvas.SetTop(groupItem, groupItemTop + delta);
                groupItem.Height = groupItem.ActualHeight * scale;
            }
        }

        private static void CalculateDragLimits(IEnumerable<DesignerItem> selectedItems, out double minLeft,
                                                out double minTop, out double minDeltaHorizontal,
                                                out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            // drag limits are set by these parameters: canvas top, canvas left, minHeight, minWidth
            // calculate min value for each parameter for each item
            foreach (var item in selectedItems)
            {
                var left = Canvas.GetLeft(item);
                var top = Canvas.GetTop(item);

                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
            }
        }

        #endregion
    }
}
