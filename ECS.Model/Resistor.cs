using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A resistor.
    /// </summary>
    public class Resistor : Component
    {
        private double _resistance;
        private double _voltage;
        private double _current;

        [XmlAttribute]
        public double Resistance
        {
            get { return _resistance; }
            set
            {
                _resistance = value;
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