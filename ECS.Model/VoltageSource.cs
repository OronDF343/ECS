using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A voltage source.
    /// </summary>
    public class VoltageSource : Component
    {
        /// <summary>
        /// Gets the voltage of the voltage source, in volts.
        /// </summary>
        [XmlAttribute]
        public double Voltage { get; set; }

        /// <summary>
        /// Gets or sets the total current darawn from this voltage source, in amperes.
        /// </summary>
        [XmlAttribute]
        public double Current { get; set; }
    }
}