using ECS.ViewModel;

namespace ECS
{
    /// <summary>
    /// Interaction logic for ResultDiagramView.xaml
    /// </summary>
    public partial class ResultDiagramView
    {
        public ResultDiagramView(ResultDiagramViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
