using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ECS.ViewModel
{
    /// <summary>
    ///     Locates all ViewModels in the app.
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<DesignerViewModel>();
        }

        /// <summary>
        ///     Gets the DesignerViewModel property.
        /// </summary>
        public DesignerViewModel DesignerViewModel => ServiceLocator.Current.GetInstance<DesignerViewModel>();

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup() { }
    }
}
