using System.Collections.Generic;
using System.Xml.Serialization;
using ECS.Model;
using JetBrains.Annotations;

namespace ECS.Xml
{
    /// <summary>
    /// A circuit which can be serialized to XML.
    /// </summary>
    public class CircuitXml
    {
        /// <summary>
        /// Gets a list of <see cref="Node"/>s.
        /// </summary>
        [XmlArray, XmlArrayItem(nameof(Node)), NotNull]
        public List<Node> Nodes { get; }

        /// <summary>
        /// Gets a list of <see cref="Resistor"/>s.
        /// </summary>
        [XmlArray, XmlArrayItem(nameof(Resistor)), NotNull]
        public List<Resistor> Resistors { get; }

        /// <summary>
        /// Gets a list of <see cref="VoltageSource"/>s.
        /// </summary>
        [XmlArray, XmlArrayItem(nameof(VoltageSource)), NotNull]
        public List<VoltageSource> VoltageSources { get; }

        /// <summary>
        /// Gets a list of <see cref="Link"/>s.
        /// </summary>
        [XmlArray, XmlArrayItem(nameof(Link)), NotNull]
        public List<Link> Links { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CircuitXml()
        {
            Nodes = new List<Node>();
            Resistors = new List<Resistor>();
            VoltageSources = new List<VoltageSource>();
            Links = new List<Link>();
        }
    }
}
