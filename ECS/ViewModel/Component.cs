namespace ECS.ViewModel
{
    public class Component : DiagramObject
    {
        private Node _node1;
        public Node Node1
        {
            get { return _node1; }
            set
            {
                _node1 = value;
                OnPropertyChanged();
            }
        }

        private Node _node2;
        public Node Node2
        {
            get { return _node2; }
            set
            {
                _node2 = value;
                OnPropertyChanged();
            }
        }
    }
}
