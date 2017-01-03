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

            /* we only move DesignerItems
            var designerItems = (from i in designer.SelectionService.CurrentSelection.OfType<DesignerItem>()
                                 let leftR = Canvas.GetLeft(i)
                                 let topR = Canvas.GetTop(i)
                                 select new
                                 {
                                     Item = i,
                                     Left = double.IsNaN(leftR) ? 0 : leftR,
                                     Top = double.IsNaN(topR) ? 0 : topR
                                 }).ToList();*/
            
            var left = Canvas.GetLeft(designerItem);
            var top = Canvas.GetTop(designerItem);
            left = double.IsNaN(left) ? 0 : left;
            top = double.IsNaN(top) ? 0 : top;

            /*var minLeft = designerItems.Min(i => i?.Left) ?? double.MaxValue;
            var minTop = designerItems.Min(i => i?.Top) ?? double.MaxValue;*/

            var deltaHorizontal = Math.Max(-left, e.HorizontalChange);
            var deltaVertical = Math.Max(-top, e.VerticalChange);

            /*designerItems.ForEach(i =>
                                  {
                                      Canvas.SetLeft(i.Item, i.Left + deltaHorizontal);
                                      Canvas.SetTop(i.Item, i.Top + deltaVertical);
                                  });*/

            Canvas.SetLeft(designerItem, left + deltaHorizontal);
            Canvas.SetTop(designerItem, top + deltaVertical);

            designer.InvalidateMeasure();
            e.Handled = true;
        }
    }
}
