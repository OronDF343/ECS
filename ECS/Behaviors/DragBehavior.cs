using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using ECS.Model;

namespace ECS.Behaviors
{
    public class DragBehavior : Behavior<Thumb>
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(DragBehavior),
                                        new PropertyMetadata(false));

        public bool IsEnabled
        {
            get => GetValue(IsEnabledProperty) as bool? ?? false;
            set => SetValue(IsEnabledProperty, value);
        }

        protected override void OnAttached()
        {
            if (AssociatedObject == null) return;
            AssociatedObject.DragDelta += AssociatedObject_DragDelta;
            base.OnAttached();
        }

        private void AssociatedObject_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = AssociatedObject?.DataContext as DiagramObject;
            if (e == null || !IsEnabled || elem == null) return;
            elem.X = Math.Max(elem.X + e.HorizontalChange, 0);
            elem.Y = Math.Max(elem.Y + e.VerticalChange, 0);
            // TODO: Prevent OOB at bottom/right of canvas?
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject == null) return;
            AssociatedObject.DragDelta -= AssociatedObject_DragDelta;
            base.OnDetaching();
        }
    }
}
