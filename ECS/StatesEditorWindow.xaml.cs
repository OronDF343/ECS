using System.Windows;
using ECS.ViewModel;

namespace ECS
{
    /// <summary>
    /// Interaction logic for StatesEditorWindow.xaml
    /// </summary>
    public partial class StatesEditorWindow : Window
    {
        public StatesEditorWindow(StatesEditorViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
