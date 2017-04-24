using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace ECS.ViewModel
{
    public class ResultsViewModel : ViewModelBase
    {
        public ResultsViewModel(ObservableCollection<TabItem> diagramViews)
        {
            Tabs = diagramViews;
        }

        public ObservableCollection<TabItem> Tabs { get; }
    }
}
