using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public class Node : DiagramObject
    {
        private double _voltage;

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
