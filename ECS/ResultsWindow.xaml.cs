using System.Windows;
using ECS.ViewModel;

namespace ECS
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(ResultsViewModel results)
        {
            DataContext = results;
            InitializeComponent();
        }
    }
}
