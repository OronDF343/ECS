using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using ECS.Model;
using JetBrains.Annotations;
using Serilog;

namespace ECS.Layout
{
    public class DesignerCanvas : Canvas
    {
        public DesignerCanvas()
        {
            _items = new Dictionary<object, DesignerItem>();
        }

        private Point? _rubberbandSelectionStartPoint;

        private SelectionService _selectionService;

        [NotNull]
        internal SelectionService SelectionService
            => _selectionService ?? (_selectionService = new SelectionService(this));

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Equals(e.Source, this)) return;
            // in case that this click is the start of a 
            // drag operation we cache the start point
            _rubberbandSelectionStartPoint = e.GetPosition(this);

            // if you click directly on the canvas all 
            // selected items are 'de-selected'
            SelectionService.ClearSelection();
            Focus();
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed) _rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (_rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);
                    adornerLayer.Add(adorner);
                }
            }
            e.Handled = true;
        }

        /*protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var dragObject = e.Data?.GetData(typeof(DragObject)) as DragObject;
            if (string.IsNullOrEmpty(dragObject?.Xaml)) return;
            var content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

            if (content != null)
            {
                var newItem = new DesignerItem { Content = content };

                var position = e.GetPosition(this);

                if (dragObject.DesiredSize.HasValue)
                {
                    var desiredSize = dragObject.DesiredSize.Value;
                    newItem.Width = desiredSize.Width;
                    newItem.Height = desiredSize.Height;

                    SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                    SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
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
        }*/

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
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height)) continue;
                size.Width = Math.Max(size.Width, left + desiredSize.Width);
                size.Height = Math.Max(size.Height, top + desiredSize.Height);
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (!item.ApplyTemplate() || !(item.Content is UIElement)) return;
            var template = Controls.DesignerItem.GetConnectorDecoratorTemplate((UIElement)item.Content);
            var decorator = item.Template?.FindName("PART_ConnectorDecorator", item) as Control;
            if ((decorator != null) && (template != null)) decorator.Template = template;
        }
        
        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DesignerCanvas),
                                          new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DesignerCanvas;
            if (c == null) return;

            var l = e.OldValue as IEnumerable;
            if (l != null) foreach (var i in l) c.AddItem(i);

            var ol = l as INotifyCollectionChanged;
            if (ol != null) ol.CollectionChanged -= c.ObservableCollectionChanged;
            
            var n = e.NewValue as IEnumerable;
            if (n != null) foreach (var i in n) c.RemoveItem(i);

            var on = n as INotifyCollectionChanged;
            if (on != null) on.CollectionChanged += c.ObservableCollectionChanged;
        }

        [NotNull]
        private readonly Dictionary<object, DesignerItem> _items;

        private void ObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var i in _items) RemoveItem(i);
                foreach (var i in (IEnumerable)sender) AddItem(i);
            }
            else
            {
                if (args.OldItems != null) foreach (var i in args.OldItems) RemoveItem(i);
                if (args.NewItems != null) foreach (var i in args.NewItems) AddItem(i);
            }
        }

        private void AddItem(object o)
        {
            if (o == null)
            {
                Log.Warning("Null object detected in designer!");
                return;
            }
            var i = new DesignerItem { DataContext = o };
            if (Resources != null)
            {
                var dt = (DataTemplate)Resources[new DataTemplateKey(o.GetType())];
                i.Content = dt.LoadContent();
            }
            _items.Add(o, i);
            var dobj = o as DiagramObject;
            if (dobj != null)
            {
                BindingOperations.SetBinding(i, LeftProperty, new Binding(nameof(DiagramObject.X)) { Source = dobj, Mode = BindingMode.TwoWay });
                BindingOperations.SetBinding(i, TopProperty, new Binding(nameof(DiagramObject.Y)) { Source = dobj, Mode = BindingMode.TwoWay });
            }
            Children.Add(i);
        }

        private void RemoveItem(object o)
        {
            if (o == null || !_items.ContainsKey(o)) return;
            var i = _items[o];
            _items.Remove(o);
            var dobj = o as DiagramObject;
            if (dobj != null)
            {
                BindingOperations.ClearBinding(i, LeftProperty);
                BindingOperations.ClearBinding(i, TopProperty);
            }
            Children.Remove(i);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                if (value == null)
                {
                    ClearValue(ItemsSourceProperty);
                }
                else
                {
                    SetValue(ItemsSourceProperty, value);
                }
            }
        }
    }
}
