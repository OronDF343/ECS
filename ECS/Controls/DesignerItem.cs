using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Controls
{
    public class DesignerItem : ContentControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool),
                                        typeof(DesignerItem),
                                        new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MoveThumbTemplateProperty =
            DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetMoveThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element?.GetValue(MoveThumbTemplateProperty);
        }

        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
        {
            element?.SetValue(MoveThumbTemplateProperty, value);
        }

        static DesignerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem),
                                                     new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem()
        {
            Loaded += DesignerItem_Loaded;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            var designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None) IsSelected = !IsSelected;
                else
                {
                    if (!IsSelected)
                    {
                        designer.DeselectAll();
                        IsSelected = true;
                    }
                }
            }

            e.Handled = false;
        }

        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (Template == null) return;
            var contentPresenter = Template.FindName("PART_ContentPresenter", this) as ContentPresenter;

            var thumb = Template.FindName("PART_MoveThumb", this) as MoveThumb;

            if (contentPresenter == null || thumb == null) return;
            var contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;

            if (contentVisual == null) return;
            var template = GetMoveThumbTemplate(contentVisual);

            if (template != null) thumb.Template = template;
        }
    }
}
