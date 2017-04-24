using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class ResultDiagramSnapshotViewModel : ViewModelBase
    {
        public ResultDiagramSnapshotViewModel(Brush br, string name)
        {
            Brush = br;
            Name = name;
        }

        public Brush Brush { get; }
        public string Name { get; }
    }
}
