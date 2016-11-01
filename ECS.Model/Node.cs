using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public class Node : DiagramObject
    {
        private bool _isHighlighted;
        private double _voltage;

        [XmlIgnore]
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
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
