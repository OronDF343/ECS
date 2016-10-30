using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using ECS.ViewModel;

namespace ECS.Behaviors
{
    public class DragBehavior : Behavior<Thumb>
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(DragBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            AssociatedObject.DragDelta += AssociatedObject_DragDelta; 
            base.OnAttached();
        }

        private void AssociatedObject_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = AssociatedObject.DataContext as DiagramObject;
            if (e == null || !IsEnabled || elem == null) return;
            elem.X = Math.Max(elem.X + e.HorizontalChange, 0);
            elem.Y = Math.Max(elem.Y + e.VerticalChange, 0);
            // TODO: Prevent OOB at bottom/right of canvas?
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragDelta -= AssociatedObject_DragDelta;
            base.OnDetaching();
        }

        public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
    }
}
