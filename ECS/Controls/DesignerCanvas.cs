using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace ECS.Controls
{
    public class DesignerCanvas : Canvas 
    {
        private Point? _dragStartPoint;

        public IEnumerable<DesignerItem> SelectedItems => from item in Children.OfType<DesignerItem>()
                                                          where item.IsSelected
                                                          select item;

        public void DeselectAll()
        {
            foreach (var item in SelectedItems) item.IsSelected = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Equals(e.Source, this)) return;
            _dragStartPoint = e.GetPosition(this);
            DeselectAll();
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed) _dragStartPoint = null;

            if (!_dragStartPoint.HasValue) return;
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                var adorner = new RubberbandAdorner(this, _dragStartPoint);
                adornerLayer.Add(adorner);
            }

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var xamlString = e.Data.GetData("DESIGNER_ITEM") as string;
            if (string.IsNullOrEmpty(xamlString)) return;
            var content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as FrameworkElement;

            if (content != null)
            {
                var newItem = new DesignerItem { Content = content };

                var position = e.GetPosition(this);
                if (content.MinHeight != 0 && content.MinWidth != 0)
                {
                    newItem.Width = content.MinWidth * 2; ;
                    newItem.Height = content.MinHeight * 2;
                }
                else
                {
                    newItem.Width = 65;
                    newItem.Height = 65;
                }
                SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                Children.Add(newItem);

                DeselectAll();
                newItem.IsSelected = true;
            }

            e.Handled = true;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = new Size();
            foreach (UIElement element in Children)
            {
                var left = GetLeft(element);
                var top = GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                element.Measure(constraint);

                var desiredSize = element.DesiredSize;
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height)) continue;
                size.Width = Math.Max(size.Width, left + desiredSize.Width);
                size.Height = Math.Max(size.Height, top + desiredSize.Height);
            }

            // add some extra margin
            //size.Width += 10;
            //size.Height += 10;
            return size;
        }

    }
}
