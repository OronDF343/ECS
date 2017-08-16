using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ECS.Layout
{
    // Names of templated parts
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb)),
     TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable
    {
        static DesignerItem()
        {
            // Change to our default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem),
                                                     new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem()
        {
            Loaded += DesignerItem_Loaded;
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                                        typeof(bool),
                                        typeof(DesignerItem),
                                        new FrameworkPropertyMetadata(false));

        // Set default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            var contentPresenter = Template?.FindName("PART_ContentPresenter", this) as ContentPresenter;
            if (contentPresenter == null) return;
            var contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
            if (contentVisual == null) return;
            var thumb = Template.FindName("PART_DragThumb", this) as DragThumb;
            if (thumb == null) return;
            var template = GetDragThumbTemplate(contentVisual);
            if (template != null) thumb.Template = template;
        }

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }
    }
}
