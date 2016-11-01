namespace ECS.ViewModel
{
    public class Node : DiagramObject
    {
        private bool _isHighlighted;
        private double _voltage;

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public double Voltage
        {
            get { return _voltage; }
            set
            {
                _voltage = value;
                OnPropertyChanged();
            }
        }
    }
}
