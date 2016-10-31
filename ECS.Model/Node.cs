using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A junction between two or more circuit components.
    /// </summary>
    public class Node
    {
        /// <inheritdoc/>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the voltage at this node.
        /// </summary>
        [XmlAttribute]
        public double Voltage { get; set; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Node && obj.GetHashCode() == GetHashCode();
        }
    }
}