using System.Xml.Serialization;
using ECS.Core.Model;

namespace ECS.Model
{
    /// <summary>
    ///     A voltage source.
    /// </summary>
    public class VoltageSource : Component, IVoltageSource
    {
        public VoltageSource() { }

        /// <summary>
        ///     Creates a new <see cref="VoltageSource" /> with a given voltage.
        /// </summary>
        /// <param name="v">The voltage of the voltage source, in volts.</param>
        public VoltageSource(double v)
        {
            Voltage = v;
        }

        private double _current;
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
