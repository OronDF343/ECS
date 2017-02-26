using System;
using System.Xml.Serialization;
using ECS.Core.Model;

namespace ECS.Model
{
    /// <summary>
    ///     A resistor.
    /// </summary>
    public class Resistor : Component, IResistor
    {
        public Resistor() { }

        /// <summary>
        ///     Creates a new <see cref="Resistor" /> with a given resistance.
        /// </summary>
        /// <param name="r">The resistance of the resistor, in ohms. Must be greater than 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="r" /> is 0 or less.</exception>
        public Resistor(double r)
        {
            if (r <= 0)
                throw new ArgumentOutOfRangeException(nameof(r), r, "Resistance must be greater than 0!");
            Resistance = r;
        }

        private double _current;
        private double _resistance;
        private double _voltage;

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

        [XmlIgnore]
        public double Conductance => 1.0 / Resistance;
    }
}
