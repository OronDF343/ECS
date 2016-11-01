using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A voltage source.
    /// </summary>
    public class VoltageSource : Component
    {
        private double _voltage;
        private double _current;

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

        [XmlAttribute]
        public double Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged();
            }
        }
    }
}