using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ECS.Controls;

namespace ECS.Behaviors
{
    public class DragBehavior : Behavior<DesignerItem>
    {
        protected override void OnAttached()
        {
            AssociatedObject.DragDelta += AssociatedObject_DragDelta; 
            base.OnAttached();
        }

        private void AssociatedObject_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (e == null) return;

            var left = Canvas.GetLeft(AssociatedObject);
            var top = Canvas.GetTop(AssociatedObject);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            Canvas.SetLeft(AssociatedObject, Math.Max(left + e.HorizontalChange, 0));
            Canvas.SetTop(AssociatedObject, Math.Max(top + e.VerticalChange, 0));
            // TODO: Prevent OOB at bottom/right of canvas?
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragDelta -= AssociatedObject_DragDelta;
            base.OnDetaching();
        }
    }
}
