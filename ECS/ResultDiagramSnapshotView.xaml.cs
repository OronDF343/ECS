using ECS.ViewModel;

namespace ECS
{
    /// <summary>
    /// Interaction logic for ResultDiagramSnapshotView.xaml
    /// </summary>
    public partial class ResultDiagramSnapshotView
    {
        public ResultDiagramSnapshotView(ResultDiagramSnapshotViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
