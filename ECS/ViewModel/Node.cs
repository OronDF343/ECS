using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace ECS.ViewModel
{
    public class Node : DiagramObject
    {
        public Node()
        {
            ComponentLinks = new ObservableCollection<ComponentLink>();
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public ObservableCollection<ComponentLink> ComponentLinks { get; }
    }
}
