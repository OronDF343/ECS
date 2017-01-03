using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace ECS.Converters
{
    public class ClickEventToPointConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            return (value as MouseButtonEventArgs)?.GetPosition(parameter as IInputElement);
        }
    }
}