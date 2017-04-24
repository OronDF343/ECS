using ECS.ViewModel;

namespace ECS
{
    /// <summary>
    /// Interaction logic for ResultErrorView.xaml
    /// </summary>
    public partial class ResultErrorView
    {
        public ResultErrorView(ResultErrorViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
