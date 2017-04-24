using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            _items = new Dictionary<DiagramObject, DesignerItem>();
        }

        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DesignerCanvas),
                                          new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register("SelectedItem", typeof(object), typeof(DesignerCanvas),
                                          new FrameworkPropertyMetadata(OnSelectedItemChanged, VerifySelectedItem));

        [NotNull]
        private readonly Dictionary<DiagramObject, DesignerItem> _items;

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                if (value == null) ClearValue(ItemsSourceProperty);
                else SetValue(ItemsSourceProperty, value);
            }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Equals(e.Source, this)) return;

            Focus();
            //e.Handled = true;
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

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DesignerCanvas;
            if (c == null) return;

            var l = e.OldValue as IEnumerable<DiagramObject>;
            if (l != null) foreach (var i in l) c.AddItem(i);

            var ol = l as INotifyCollectionChanged;
            if (ol != null) ol.CollectionChanged -= c.ObservableCollectionChanged;

            var n = e.NewValue as IEnumerable<DiagramObject>;
            if (n != null) foreach (var i in n) c.RemoveItem(i);

            var on = n as INotifyCollectionChanged;
            if (on != null) on.CollectionChanged += c.ObservableCollectionChanged;
        }

        private void ObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var p in _items)
                {
                    if (p.Key != null && p.Value != null)
                    {
                        BindingOperations.ClearBinding(p.Value, LeftProperty);
                        BindingOperations.ClearBinding(p.Value, TopProperty);
                    }
                    Children.Remove(p.Value);
                }
                _items.Clear();
                foreach (var i in (IEnumerable)sender) AddItem(i as DiagramObject);
            }
            else
            {
                if (args.OldItems != null) foreach (var i in args.OldItems) RemoveItem(i as DiagramObject);
                if (args.NewItems != null) foreach (var i in args.NewItems) AddItem(i as DiagramObject);
            }
        }

        private void AddItem(DiagramObject o)
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
            var dobj = o;
            if (dobj != null)
            {
                BindingOperations.SetBinding(i, LeftProperty,
                                             new Binding(nameof(DiagramObject.X))
                                             {
                                                 Source = dobj,
                                                 Mode = BindingMode.TwoWay
                                             });
                BindingOperations.SetBinding(i, TopProperty,
                                             new Binding(nameof(DiagramObject.Y))
                                             {
                                                 Source = dobj,
                                                 Mode = BindingMode.TwoWay
                                             });
            }
            Children.Add(i);
        }

        private void RemoveItem(DiagramObject o)
        {
            if (o == null || !_items.ContainsKey(o)) return;
            var i = _items[o];
            _items.Remove(o);
            var dobj = o;
            if (dobj != null)
            {
                BindingOperations.ClearBinding(i, LeftProperty);
                BindingOperations.ClearBinding(i, TopProperty);
            }
            Children.Remove(i);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DesignerCanvas) || e.OldValue == null) return;
            var dc = (DesignerCanvas)d;
            DesignerItem i;
            if (e.OldValue is DiagramObject && dc._items.TryGetValue((DiagramObject)e.OldValue, out i)) i.IsSelected = false;
        }

        private static object VerifySelectedItem(DependencyObject d, object basevalue)
        {
            if (basevalue != null && (d as DesignerCanvas)?.ItemsSource.OfType<object>().Contains(basevalue) != true) throw new ArgumentException("Can't select non-existent item!");
            return basevalue;
        }
    }
}
