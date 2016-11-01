namespace ECS.ViewModel
{
    public abstract class Component : DiagramObject
    {
        private Node _node1;
        private Node _node2;
        private double _rotation;

        public Node Node1
        {
            get { return _node1; }
            set
            {
                _node1 = value;
                OnPropertyChanged();
            }
        }

        public Node Node2
        {
            get { return _node2; }
            set
            {
                _node2 = value;
                OnPropertyChanged();
            }
        }

        public double Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                OnPropertyChanged();
            }
        }
    }
}