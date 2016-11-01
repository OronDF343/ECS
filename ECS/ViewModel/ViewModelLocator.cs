using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
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

            SimpleIoc.Default.Register<WelcomeViewModel>();
        }

        /// <summary>
        ///     Gets the Designer property.
        /// </summary>
        [CanBeNull]
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public DesignerViewModel Designer => ServiceLocator.Current.GetInstance<DesignerViewModel>();

        /// <summary>
        ///     Gets the Designer property.
        /// </summary>
        [CanBeNull]
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public WelcomeViewModel Welcome => ServiceLocator.Current.GetInstance<WelcomeViewModel>();

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup() { }
    }
}
