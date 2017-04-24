using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ECS.Layout
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb)),
     TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control)),
     TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable
    {
        static DesignerItem()
        {
            // set the key to reference the style for this control
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

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate),
                                                typeof(DesignerItem));

        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                        typeof(bool),
                                        typeof(DesignerItem),
                                        new FrameworkPropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            var designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            
            if (designer != null)
            {
                designer.SelectedItem = DataContext;
                IsSelected = true;
                Focus();
            }

            e.Handled = false;
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
