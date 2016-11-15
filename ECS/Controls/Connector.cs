using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Controls
{
    public class Connector : Control, INotifyPropertyChanged
    {
        // keep track of connections that link to this connector
        private List<Connection> _connections;
        // drag start point, relative to the DesignerCanvas
        private Point? _dragStartPoint;

        // the DesignerItem this Connector belongs to;
        // retrieved from DataContext, which is set in the
        // DesignerItem template
        private DesignerItem _parentDesignerItem;

        // center position of this Connector relative to the DesignerCanvas
        private Point _position;

        public Connector()
        {
            // fired when layout changes
            LayoutUpdated += Connector_LayoutUpdated;
        }

        public ConnectorOrientation Orientation { get; set; }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        public DesignerItem ParentDesignerItem => _parentDesignerItem ?? (_parentDesignerItem = DataContext as DesignerItem);

        public List<Connection> Connections => _connections ?? (_connections = new List<Connection>());

        // when the layout changes we update the position property
        private void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            var designer = GetDesignerCanvas(this);
            if (designer != null)
                Position = TransformToAncestor(designer).Transform(new Point(Width/2, Height/2));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var canvas = GetDesignerCanvas(this);
            if (canvas == null) return;
            // position relative to DesignerCanvas
            _dragStartPoint = e.GetPosition(canvas);
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                _dragStartPoint = null;

            // but if mouse button is pressed and start point value is set we do have one
            if (!_dragStartPoint.HasValue) return;
            // create connection adorner 
            var canvas = GetDesignerCanvas(this);
            if (canvas == null) return;
            var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
            if (adornerLayer == null) return;
            var adorner = new ConnectorAdorner(canvas, this);
            adornerLayer.Add(adorner);
            e.Handled = true;
        }

        internal ConnectorInfo GetInfo()
        {
            var info = new ConnectorInfo
            {
                DesignerItemLeft = Canvas.GetLeft(ParentDesignerItem),
                DesignerItemTop = Canvas.GetTop(ParentDesignerItem),
                DesignerItemSize = new Size(ParentDesignerItem.ActualWidth, ParentDesignerItem.ActualHeight),
                Orientation = Orientation,
                Position = Position
            };
            return info;
        }

        // iterate through visual tree to get parent DesignerCanvas
        private DesignerCanvas GetDesignerCanvas(DependencyObject element)
        {
            while ((element != null) && !(element is DesignerCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as DesignerCanvas;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    // provides compact info about a connector; used for the 
    // routing algorithm, instead of hand over a full fledged Connector
    internal struct ConnectorInfo
    {
        public double DesignerItemLeft { get; set; }
        public double DesignerItemTop { get; set; }
        public Size DesignerItemSize { get; set; }
        public Point Position { get; set; }
        public ConnectorOrientation Orientation { get; set; }
    }

    public enum ConnectorOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }
}