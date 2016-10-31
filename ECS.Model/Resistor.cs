using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A resistor.
    /// </summary>
    public class Resistor : Component
    {
        /// <summary>
        /// Gets the resistance of the resistor, in ohms.
        /// </summary>
        [XmlAttribute]
        public double Resistance { get; set; }

        /// <summary>
        /// Gets the voltage at the resistor, in volts.
        /// </summary>
        [XmlAttribute]
        public double Voltage { get; set; }

        /// <summary>
        /// Gets the current on the resistor, in amperes.
        /// </summary>
        [XmlAttribute]
        public double Current { get; set; }
    }
}