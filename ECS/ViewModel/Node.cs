namespace ECS.ViewModel
{
    public class Node : DiagramObject
    {
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
    }
}
