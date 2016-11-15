using System;
using System.IO;
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
        private Point? _rubberbandSelectionStartPoint;

        private SelectionService _selectionService;

        internal SelectionService SelectionService
        {
            get
            {
                if (_selectionService == null)
                    _selectionService = new SelectionService(this);

                return _selectionService;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start of a 
                // drag operation we cache the start point
                _rubberbandSelectionStartPoint = e.GetPosition(this);

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                _rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (_rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);
                    if (adorner != null)
                        adornerLayer.Add(adorner);
                }
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if ((dragObject != null) && !string.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                var content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    newItem = new DesignerItem();
                    newItem.Content = content;

                    var position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        var desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        SetLeft(newItem, Math.Max(0, position.X - newItem.Width/2));
                        SetTop(newItem, Math.Max(0, position.Y - newItem.Height/2));
                    }
                    else
                    {
                        SetLeft(newItem, Math.Max(0, position.X));
                        SetTop(newItem, Math.Max(0, position.Y));
                    }

                    SetZIndex(newItem, Children.Count);
                    Children.Add(newItem);
                    SetConnectorDecoratorTemplate(newItem);

                    //update selection
                    SelectionService.SelectItem(newItem);
                    newItem.Focus();
                }

                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = new Size();

            foreach (UIElement element in InternalChildren)
            {
                var left = GetLeft(element);
                var top = GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                var desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement)
            {
                var template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                var decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if ((decorator != null) && (template != null))
                    decorator.Template = template;
            }
        }
    }
}